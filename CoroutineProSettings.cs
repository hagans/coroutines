using UnityEngine.Events;

namespace Hagans.Coroutines
{
    using Editor;

    [System.Serializable]
    class CoroutineProSettings
    {
        public enum EnumerableType
        {
            HashSet,
            List,
            Array
        }

        public UnityEvent
            onStart = new UnityEvent(),
            onComplete = new UnityEvent(),
            onCancel = new UnityEvent(),
            onPause = new UnityEvent(),
            onResume = new UnityEvent(),
            onDestroy = new UnityEvent();

        public bool editorEventsMenuActive = false;

        public bool destroyOnEnd = false;

        public EnumerableType enumerableType = EnumerableType.HashSet;

        public int arrayLenght;
        
        static CoroutineProSettings _default;


        public static CoroutineProSettings Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new CoroutineProSettings();
                    SaveableEditor.TryGetValue<CoroutineProEditorWindow, CoroutineProSettings>(ref _default);
                }
                return _default;
            }
        }
    }
}

