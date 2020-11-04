using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// <summary>
// Extended pausable and restartable <see cref="Coroutine"/>. It includes events and persistency through <see cref="GameObject"/> disables and scene changes.
// </summary>
namespace Hagans.Coroutines
{
    /// <summary>
    /// Extended pausable and restartable <see cref="Coroutine"/>. It includes events and persistency through <see cref="GameObject"/> disables and scene changes.
    /// </summary>
    public class CoroutinePro : CustomYieldInstruction
    {
        class UnityEventWithDefaultValues : UnityEvent
        {
            public UnityEventWithDefaultValues(UnityEvent byDefault) => _byDefault = byDefault;

            UnityEvent _byDefault;

            public new void Invoke()
            {
                _byDefault.Invoke();
                base.Invoke();
            }
        }

        enum CoroutineProState
        {
            Idle,
            Paused,
            Running
        }

        /// <summary>
        /// Creates a new instance of <see cref="CoroutinePro"/>.
        /// </summary>
        /// <param name="routine"><see cref="IEnumerable"/> to execute.</param>
        /// <param name="name">Name identifier of the <see cref="CoroutinePro"/> instance.</param>
        /// <param name="caller"><see cref="MonoBehaviour"/> related to this <see cref="CoroutinePro"/></param>
        public CoroutinePro(IEnumerable routine, string name = null, MonoBehaviour caller = null)
        {
            _model = routine;
            Name = name;
            _caller = caller;
            Add(this);
        }


        CoroutineProState _state = CoroutineProState.Idle;

        readonly IEnumerable _model;
        IEnumerator _runTime;
        MonoBehaviour _caller;
        float _pauseDelay;
        bool _destroyOnEnd = CoroutineProSettings.Default.destroyOnEnd;

        UnityEventWithDefaultValues
            _onStart = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onStart),
            _onPause = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onPause),
            _onResume = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onResume),
            _onStop = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onCancel),
            _onComplete = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onComplete),
            _onDestroy = new UnityEventWithDefaultValues(CoroutineProSettings.Default.onDestroy);

        /// <summary>
        /// Enumerable with all the current <see cref="CoroutinePro"/> instances.
        /// </summary>
        public static IEnumerable<CoroutinePro> Coroutines { get; private set; } = Create();

        /// <summary>
        /// Identifier name of the <see cref="CoroutinePro"/> instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> starts.
        /// </summary>
        public UnityEvent OnStart => _onStart;

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> is paused.
        /// </summary>
        public UnityEvent OnPause => _onPause;

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> is resumed.
        /// </summary>
        public UnityEvent OnResume => _onResume;

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> is canceled.
        /// </summary>
        public UnityEvent OnCancel => _onStop;

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> is completed.
        /// </summary>
        public UnityEvent OnComplete => _onComplete;

        /// <summary>
        /// Called when <see cref="CoroutinePro"/> is destroyed.
        /// </summary>
        public UnityEvent OnDestroy => _onDestroy;

        /// <summary>
        /// True if <see cref="CoroutinePro"/> instance is paused. Otherwise false.
        /// </summary>
        public bool IsPaused
        {
            get => _state == CoroutineProState.Paused;
            set
            {
                if (value)
                {
                    Pause();
                    return;
                }
                Resume();
            }
        }

        /// <summary>
        /// True if <see cref="CoroutinePro"/> instance is running. Otherwise false.
        /// </summary>
        public bool IsRunning
        {
            get => _state == CoroutineProState.Running;
            set
            {
                if (value)
                {
                    Pause();
                    return;
                }
                Resume();
            }
        }

        /// <summary>
        /// <see cref="CustomYieldInstruction"/> implementation. Allows <see cref="CoroutinePro"/>s to be called 
        /// as a yield return from another <see cref="CoroutinePro"/>.
        /// </summary>
        public override bool keepWaiting => !(_state == CoroutineProState.Idle);

        /// <summary>
        /// Returns all the <see cref="CoroutinePro"/>s which match with the specified name.
        /// </summary>
        /// <param name="name">Identifier string of the <see cref="CoroutinePro"/>s.</param>
        /// <returns><see cref="IEnumerable"/> with all the <see cref="CoroutinePro"/>s found.</returns>
        /// <exception cref="KeyNotFoundException">No <see cref="CoroutinePro"/> with the specified name.</exception>
        public static IEnumerable<CoroutinePro> Coroutine(string name)
        {
            var result = Coroutines.Where(routine => routine.Name == name);
            if (!result.Any()) throw new KeyNotFoundException("Identifier " + name + "does not match with any CoroutinePro.");
            return result;
        }

        /// <summary>
        /// All the <see cref="CoroutinePro"/>s called by the specified <see cref="MonoBehaviour"/>
        /// </summary>
        /// <param name="monoBehaviour">Instance which must match.</param>
        /// <returns><see cref="IEnumerable"/> with all the <see cref="CoroutinePro"/>s found.</returns>
        /// <exception cref="ArgumentNullException">Can't search MaxRoutine for a null <see cref="MonoBehaviour"/>.</exception>
        /// <exception cref="KeyNotFoundException">No <see cref="CoroutinePro"/> attached to this <see cref="MonoBehaviour"/>.</exception>
        public static IEnumerable<CoroutinePro> CoroutinesOf(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null) throw new ArgumentNullException(nameof(monoBehaviour), "Can't search MaxRoutine for a null MonoBehaviour.");
            var result = Coroutines.Where(routine => routine != null && routine._caller == monoBehaviour);
            if (!result.Any()) throw new KeyNotFoundException("MonoBehaviour " + monoBehaviour.GetType().Name + " at " + monoBehaviour.gameObject.name + " has not attached any CoroutinePro.");
            return result;
        }

        /// <summary>
        /// Starts an Idle <see cref="CoroutinePro"/>
        /// </summary>
        /// /// <exception cref="InvalidOperationException"><see cref="CoroutinePro"/> is already started.</exception>
        public void Start()
        {
            if (_state != CoroutineProState.Idle) throw new InvalidOperationException("RoutineMax is still started, use Reset() instead.");
            if (_caller == null)
            {
                var caller = new GameObject("Coroutine Pro Controller " + Name, typeof(CoroutineProController)).GetComponent<CoroutineProController>();
                caller.Initialize(this);
                caller.StartCoroutine(Iterate());
            }
            else _caller.StartCoroutine(Iterate());
        }

        /// <summary>
        /// Starts an idle persistent <see cref="CoroutinePro"/>. Persistent <see cref="CoroutinePro"/> will not stop 
        /// between Scenes or when it <see cref="CoroutineProMonoBehaviour"/> instance is destroyed or disabled.               
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="CoroutinePro"/> is already started.</exception>
        public void StartPersistent()
        {
            if (_state != CoroutineProState.Idle) throw new InvalidOperationException("CoroutinePro is already started, use Reset() instead.");
            CoroutineProManager.Instance.StartCoroutine(Iterate());
        }

        /// <summary>
        /// Stops a <see cref="CoroutinePro"/> execution.
        /// </summary>
        public virtual void Stop()
        {
            if (_state != CoroutineProState.Idle) _onStop.Invoke();
            _state = CoroutineProState.Idle;
        }

        /// <summary>
        /// Resets a <see cref="CoroutinePro"/> execution.
        /// </summary>
        public override void Reset()
        {
            if (_state != CoroutineProState.Idle)
            {
                _runTime = _model.GetEnumerator();
                _pauseDelay = 0;
                return;
            }
        }

        /// <summary>
        /// Pauses a <see cref="CoroutinePro"/> execution.
        /// </summary>
        public virtual void Pause()
        {
            if (_state == CoroutineProState.Running)
            {
                _onPause.Invoke();
                _state = CoroutineProState.Paused;
                _pauseDelay = Time.time;
            }
        }

        /// <summary>
        /// Resumes a <see cref="CoroutinePro"/> execution.
        /// </summary>
        public virtual void Resume()
        {
            if (_state == CoroutineProState.Paused)
            {
                _onResume.Invoke();
                _state = CoroutineProState.Running;
            }
        }

        /// <summary>
        /// Switchs whether to pause or to resume a <see cref="CoroutinePro"/> execution.
        /// </summary>
        public void Toggle()
        {
            if (IsPaused)
            {
                Resume();
                return;
            }
            Pause();
        }

        /// <summary>
        /// Destroy all this instance references.
        /// </summary>
        public void Destroy()
        {
            Stop();
            _onDestroy.Invoke();
            Remove(this);
        }
        
        static void Add(CoroutinePro coroutine)
        {
            switch (Coroutines)
            {
                case CoroutinePro[] array:
                    for(int i = 0; i < array.Length; i++)
                    {
                        if(array[i] == null)
                        {
                            array[i] = coroutine;
                            Coroutines = array;
                            return;
                        }                        
                    }
                    throw new IndexOutOfRangeException("You reached the max amount of CoroutinePro. Resize the array or use a different store mode at Coroutines Pro settings.");
                case ICollection<CoroutinePro> collection:
                    collection.Add(coroutine);
                    Coroutines = collection;
                    return;
                case Queue<CoroutinePro> queue:
                    queue.Enqueue(coroutine);
                    Coroutines = queue;
                    return;                
            }
        }
        static void Remove(CoroutinePro coroutine)
        {
            switch (Coroutines)
            {
                case CoroutinePro[] array:
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] == coroutine)
                        {
                            array[i] = null;
                            Coroutines = array;
                            return;
                        }                        
                    }
                    return;
                case ICollection<CoroutinePro> collection:
                    collection.Remove(coroutine);
                    Coroutines = collection;
                    return;
            }
        }

        static IEnumerable<CoroutinePro> Create()
        {
            switch (CoroutineProSettings.Default.enumerableType)
            {
                case CoroutineProSettings.EnumerableType.Array:
                    return new CoroutinePro[CoroutineProSettings.Default.arrayLenght];
                case CoroutineProSettings.EnumerableType.HashSet:
                    return new HashSet<CoroutinePro>();
                case CoroutineProSettings.EnumerableType.List:
                    return new List<CoroutinePro>();
                default: return null;
            }
        }

        IEnumerator Iterate()
        {
            _runTime = _model.GetEnumerator();
            _state = CoroutineProState.Running;
            _onStart.Invoke();
            while (true)
            {
                switch (_state)
                {
                    case CoroutineProState.Idle:
                        break;
                    case CoroutineProState.Paused:
                        yield return null;
                        continue;
                    case CoroutineProState.Running:
                        if (_pauseDelay != 0)
                        {
                            var wait = new WaitForSeconds(_pauseDelay);
                            _pauseDelay = 0;
                            yield return wait;
                        }
                        else
                        {
                            if (!_runTime.MoveNext())
                            {
                                _state = CoroutineProState.Idle;
                                _onComplete.Invoke();
                                if (_destroyOnEnd) Destroy();
                            }
                            yield return _runTime.Current;
                        }
                        if (_pauseDelay != 0) _pauseDelay = Time.time - _pauseDelay;
                        continue;
                }
                break;
            }

            _runTime = null;
        }
    }
}

