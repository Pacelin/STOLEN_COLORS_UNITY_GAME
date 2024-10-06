using UnityEditor;

namespace UniOwl.Editor
{
    public readonly struct EditorPrefsBool
    {
        private readonly string _editorPrefKey;
        private readonly bool _defaultValue;
        
        public bool Value
        {
            get => EditorPrefs.HasKey(_editorPrefKey) ? EditorPrefs.GetBool(_editorPrefKey) : _defaultValue;
            set => EditorPrefs.SetBool(_editorPrefKey, value);
        }

        public EditorPrefsBool(string key, bool defaultValue = true)
        {
            _editorPrefKey = key;
            _defaultValue = defaultValue;
        }

        public static implicit operator bool(EditorPrefsBool pref)
        {
            return pref.Value;
        }
    }
}