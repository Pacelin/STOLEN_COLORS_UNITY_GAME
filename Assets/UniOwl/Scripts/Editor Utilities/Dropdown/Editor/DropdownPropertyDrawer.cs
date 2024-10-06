using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniOwl.Editor
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownPropertyDrawer : PropertyDrawer
    {
        [SerializeField]
        private StyleSheet _dropdownStyleSheet;
        
        private SerializedProperty _property;
        private Foldout _foldout;
        private DropdownField _dropdown;
        private List<(string name, Type type)> _choices;

        private EditorPrefsBool _expanded;
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(property.serializedObject.targetObject, out string guid, out long localId);
            _expanded = new EditorPrefsBool($"{nameof(DropdownPropertyDrawer)}/{property.propertyPath}/{guid}/{localId}");
            
            _property = property;
            _choices = GetDropdownChoices(property);

            _foldout = CreateFoldoutHeader(property, !_expanded);
            _foldout.styleSheets.Add(_dropdownStyleSheet);
            AddChildrenPropertyFields(property, _foldout.contentContainer);
            
            return _foldout;
        }

        private static List<(string name, Type type)> GetDropdownChoices(SerializedProperty property)
        {
            var fi = property.GetPropertyFieldInfo();
            Type type = fi.FieldType.IsArray ? fi.FieldType.GetElementType() : fi.FieldType;
            var dropdownAttribute = property.GetAttribute<DropdownAttribute>();

            return TypeUtils.GetDropdownChoices(type, dropdownAttribute.IncludeSelf, dropdownAttribute.IncludeNone).ToList();
        }
        
        private Foldout CreateFoldoutHeader(SerializedProperty property, bool folded)
        {
            var foldout = new Foldout
            {
                text = property.displayName,
                value = folded,
            };
            foldout.BindProperty(property);
            foldout.RegisterValueChangedCallback(evt => _expanded.Value = !evt.newValue);

            _dropdown = CreateDropdown(property);
            
            foldout.Q<Toggle>().contentContainer.Add(_dropdown);

            return foldout;
        }

        private DropdownField CreateDropdown(SerializedProperty property)
        {
            List<string> choiceNames = _choices.Select(pair => pair.name).ToList();
            Type propType = property.GetPropertyValue()?.GetType();
            
            var dropdown = new DropdownField
            {
                label = string.Empty,
                choices = choiceNames,
            };
            dropdown.AddToClassList("dropdown");
            dropdown.RegisterValueChangedCallback(ChoiceChanged);
            dropdown.SetValueWithoutNotify(_choices.Where(pair => pair.type == propType).Select(pair => pair.name).FirstOrDefault() ?? "Name not found");
            
            return dropdown;
        }
        
        private void ChoiceChanged(ChangeEvent<string> evt)
        {
            Type oldType = _property.GetPropertyType();
            Type newType = _choices.FirstOrDefault(pair => string.Equals(pair.name, evt.newValue)).type;

            if (oldType == newType) return;

            if (newType == null)
            {
                _property.SetPropertyValue(default);
                _property.serializedObject.ApplyModifiedProperties();
                ClearChildrenPropertyFields(_foldout.contentContainer);
                return;
            }
            
            object value;

            if (typeof(ScriptableObject).IsAssignableFrom(newType))
                value = ScriptableObject.CreateInstance(newType);
            else
                value = Activator.CreateInstance(newType);

            _property.SetPropertyValue(value);
            _property.serializedObject.ApplyModifiedProperties();
            
            ClearChildrenPropertyFields(_foldout.contentContainer);
            AddChildrenPropertyFields(_property, _foldout.contentContainer);
        }

        private void ClearChildrenPropertyFields(VisualElement container)
        {
            container.Clear();
        }
        
        private void AddChildrenPropertyFields(SerializedProperty parent, VisualElement container)
        {
            foreach (SerializedProperty prop in parent.GetChildren())
            {
                var propertyField = new PropertyField(prop);
                propertyField.BindProperty(prop);
                container.Add(propertyField);
            }
        }
    }
}