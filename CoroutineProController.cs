using UnityEngine;

namespace Hagans.Coroutines
{
    class CoroutineProController : MonoBehaviour
    {
        public void Initialize(CoroutinePro coroutine)
        {
            coroutine.OnComplete.AddListener(() => { Destroy(gameObject); });
            coroutine.OnCancel.AddListener(() => { Destroy(gameObject); });
        }
    }
}
