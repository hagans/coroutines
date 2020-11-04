using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace Hagans.Coroutines
{
    using Hagans.Editor;

    class CoroutineProEditorWindow : SaveableEditorWindow<CoroutineProSettings>
    {
        [SerializeField] UnityEvent _onStart, _onCancel, _onPause, _onResume, _onComplete, _onDestroy;
        bool _editorEventsMenuActive, _destroyOnEnd;
        CoroutineProSettings.EnumerableType _enumerableType;
        int _arrayLength;

        protected override CoroutineProSettings InitialData => new CoroutineProSettings();


        [MenuItem("Tools/Coroutines Pro Settings")]
        static void Initialize()
        {
            var window = GetWindow<CoroutineProEditorWindow>();
            window.titleContent = new GUIContent("Coroutine Pro");

        }

        protected override void OnLoad(CoroutineProSettings data)
        {
            _onStart = data.onStart;
            _onCancel = data.onCancel;
            _onPause = data.onPause;
            _onResume = data.onResume;
            _onComplete = data.onComplete;
            _onDestroy = data.onDestroy;
            _editorEventsMenuActive = data.editorEventsMenuActive;
            _enumerableType = data.enumerableType;
            _arrayLength = data.arrayLenght;
            _destroyOnEnd = data.destroyOnEnd;
        }

        protected override CoroutineProSettings OnSave => new CoroutineProSettings()
        {
            onStart = _onStart,
            onCancel = _onCancel,
            onPause = _onPause,
            onResume = _onResume,
            onComplete = _onComplete,
            onDestroy = _onDestroy,
            editorEventsMenuActive = _editorEventsMenuActive,
            enumerableType = _enumerableType,
            arrayLenght = _arrayLength,
            destroyOnEnd = _destroyOnEnd
        };


        private void OnGUI()
        {
            GUILayout.Label("Coroutine Pro Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Destroy on end:", "Destroy automatically Coroutines Pro when completed or canceled. It saves memory but Coroutines Pro can't be restored after."));
            _destroyOnEnd = EditorGUILayout.Toggle(_destroyOnEnd);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Store engine:", "IEnumerable implementation used to store your Coroutines Pro data."));
            _enumerableType = (CoroutineProSettings.EnumerableType)EditorGUILayout.EnumPopup(_enumerableType);
            EditorGUILayout.EndHorizontal();           
            switch (_enumerableType)
            {
                case CoroutineProSettings.EnumerableType.HashSet:
                    EditorGUILayout.HelpBox("HashSet is usefull managing big amounts of Coroutines Pro since it uses HashCodes to identify them.", MessageType.Info);
                    break;
                case CoroutineProSettings.EnumerableType.List:
                    EditorGUILayout.HelpBox("HashSet is usefull managing small amounts of Coroutines Pro. It uses a numerated list in order to identify them.", MessageType.Info);
                    break;
                case CoroutineProSettings.EnumerableType.Array:
                    EditorGUILayout.HelpBox("Array is the fastest method of all since it doesn't have to be resized, but it fills the memory with null values ​​when instantiated and it is not resizable. Use it only if you handle few coroutines and you know with certainty the maximum number of them running at the same time.", _arrayLength <= 0 ? MessageType.Error : MessageType.Warning);
                    break;
            }
            if (_enumerableType == CoroutineProSettings.EnumerableType.Array)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Size:");
                _arrayLength = EditorGUILayout.IntField(_arrayLength);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            _editorEventsMenuActive = EditorGUILayout.BeginFoldoutHeaderGroup(_editorEventsMenuActive, new GUIContent("Default events: ", "Each coroutine is triggering this events by default."));
            if (_editorEventsMenuActive)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Window.FindProperty("_onStart"));
                EditorGUILayout.PropertyField(Window.FindProperty("_onCancel"));
                EditorGUILayout.PropertyField(Window.FindProperty("_onPause"));
                EditorGUILayout.PropertyField(Window.FindProperty("_onResume"));
                EditorGUILayout.PropertyField(Window.FindProperty("_onComplete"));
                EditorGUILayout.PropertyField(Window.FindProperty("_onDestroy"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}

