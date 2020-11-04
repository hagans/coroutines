using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hagans.Coroutines
{
    /// <summary>
    /// <see cref="MonoBehaviour"/> which uses <see cref="CoroutinePro"/>s instead of <see cref="Coroutine"/>s. Inherit from this in order to use <see cref="CoroutinePro"/> features.
    /// </summary>
    public class CoroutineProMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// All the <see cref="CoroutinePro"/> instances related to this instance.
        /// </summary>
        public IEnumerable<CoroutinePro> Coroutines => CoroutinePro.CoroutinesOf(this);


        /// <summary>
        /// Starts a regular <see cref="CoroutinePro"/>.
        /// </summary>
        /// <param name="routine">Execution to do during iteration.</param>
        /// <param name="name">Optional identifier string.</param>
        /// <returns>This <see cref="CoroutinePro"/> instance just created.</returns>
        public CoroutinePro StartCoroutine(IEnumerable routine, string name = null)
        {
            var coroutine = new CoroutinePro(routine, name, this);
            coroutine.Start();
            return coroutine;
        }

        /// <summary>
        /// Starts a persistent <see cref="CoroutinePro"/>. Persistent <see cref="CoroutinePro"/> will not stop 
        /// between Scenes or when it <see cref="CoroutineProMonoBehaviour"/> instance is destroyed or disabled.
        /// </summary>
        /// <param name="routine">Execution to do during iteration.</param>
        /// <param name="name">Optional identifier string.</param>
        /// <returns>This <see cref="CoroutinePro"/> instance just created.</returns>
        public CoroutinePro StartPersistentCoroutine(IEnumerable routine, string name = null)
        {
            var coroutine = new CoroutinePro(routine, name, this);
            coroutine.StartPersistent();
            return coroutine;
        }

        /// <summary>
        /// Empty method. Hides <see cref="MonoBehaviour"/> inherited member in order to apply <see cref="ObsoleteAttribute"/> to it.
        /// </summary>       
        [Obsolete("CoroutineProMonoBehaviour uses IEnumerable as parameter. Try StartCoroutine(IEnumerable routine, string name).")]
        public new void StartCoroutine(IEnumerator id) { }

        /// <summary>
        /// Empty method. Hides <see cref="MonoBehaviour"/> inherited member in order to apply <see cref="ObsoleteAttribute"/> to it.
        /// </summary>
        [Obsolete("CoroutineProMonoBehaviour uses IEnumerable as parameter. Try StartCoroutine(IEnumerable routine, string name).")]
        public new void StartCoroutine(string id) { }

        /// <summary>
        /// Empty method. Hides <see cref="MonoBehaviour"/> inherited member in order to apply <see cref="ObsoleteAttribute"/> to it.
        /// </summary>
        [Obsolete("CoroutineProMonoBehaviour uses IEnumerable as parameter. Try StartCoroutine(IEnumerable routine, string name).")]
        public new void StartCoroutine(string id, object value) { }
    }
}

