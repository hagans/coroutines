using UnityEngine;

namespace Hagans.Coroutines
{
    class CoroutineProManager : MonoBehaviour
    {
        static CoroutineProManager _instance;

        public static CoroutineProManager Instance
        {
            get
            {
                if (_instance == null) _instance = new GameObject("Coroutine Pro Manager", typeof(CoroutineProManager)).GetComponent<CoroutineProManager>();
                return _instance;
            }
        }

        void Awake() => DontDestroyOnLoad(this);        
    }
}