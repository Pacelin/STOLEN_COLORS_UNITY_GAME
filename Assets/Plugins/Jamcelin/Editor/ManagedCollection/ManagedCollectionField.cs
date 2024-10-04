using System;
using System.Collections.Generic;
using Jamcelin.Editor.Search;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Jamcelin.Editor.ManagedCollection
{
    public class ManagedCollectionField : VisualElement
    {
        private readonly Object _target;
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _collectionProperty;
        private readonly SerializedProperty _statesProperty;
        
        private readonly Label _label;
        private readonly VisualElement _itemsContainer;
        private readonly Button _addButton;
        
        private readonly List<ManagedCollectionItem> _items;
        private readonly Func<Type, object> _factory;

        public ManagedCollectionField(string label, Object target, SerializedObject serializedObject,
            SerializedProperty collectionProperty) : 
            this(label, target, serializedObject, collectionProperty, null, 
                Activator.CreateInstance) { }
        public ManagedCollectionField(string label, Object target, SerializedObject serializedObject,
            SerializedProperty collectionProperty, SerializedProperty statesProperty) : 
            this(label, target, serializedObject, collectionProperty, statesProperty,
                Activator.CreateInstance) { }
        public ManagedCollectionField(string label, Object target, SerializedObject serializedObject,
            SerializedProperty collectionProperty, Func<Type, object> factory) : 
            this(label, target, serializedObject, collectionProperty, null, factory) { }
        
        public ManagedCollectionField(
            string label,
            Object target,
            SerializedObject serializedObject,
            SerializedProperty collectionProperty, 
            SerializedProperty statesProperty,
            Func<Type, object> factory)
        {
            _items = new();
            _target = target;
            _serializedObject = serializedObject;
            _collectionProperty = collectionProperty;
            _statesProperty = statesProperty;
            _factory = factory;
            
            style.backgroundColor = new Color(0, 0, 0, 0.2f);
            style.marginBottom = 0;
            style.marginLeft = 0;
            style.marginRight = 0;
            style.marginTop = 5;
            style.paddingBottom = 12;
            style.paddingTop = 12;
            style.paddingLeft = 10;
            style.paddingRight = 10;
            style.borderBottomLeftRadius = 5;
            style.borderBottomRightRadius = 5;
            style.borderTopLeftRadius = 5;
            style.borderTopRightRadius = 5;
            
            _itemsContainer = new VisualElement();
            _itemsContainer.style.backgroundColor = new Color(0, 0, 0, 0.2f);
            _itemsContainer.style.marginBottom = 8;
            _itemsContainer.style.marginLeft = 0;
            _itemsContainer.style.marginRight = 0;
            _itemsContainer.style.marginTop = 0;
            _itemsContainer.style.paddingBottom = 10;
            _itemsContainer.style.paddingTop = 8;
            _itemsContainer.style.paddingLeft = 10;
            _itemsContainer.style.paddingRight = 10;
            _itemsContainer.style.borderBottomLeftRadius = 10;
            _itemsContainer.style.borderBottomRightRadius = 10;
            _itemsContainer.style.borderTopLeftRadius = 10;
            _itemsContainer.style.borderTopRightRadius = 10;
            
            _label = new Label(label);
            _label.style.unityTextAlign = TextAnchor.MiddleCenter;
            _label.style.fontSize = 14;

            _addButton = new Button(OnAddClick);
            _addButton.text = "Add";
            _addButton.style.marginBottom = 0;
            _addButton.style.marginLeft = 0;
            _addButton.style.marginRight = 0;
            _addButton.style.marginTop = 0;
            _addButton.style.paddingBottom = 5;
            _addButton.style.paddingTop = 5;
            _addButton.style.paddingLeft = 5;
            _addButton.style.paddingRight = 5;
            
            _itemsContainer.Add(_label);
            Add(_itemsContainer);
            Add(_addButton);

            for (int i = 0; i < _collectionProperty.arraySize; i++)
            {
                SerializedProperty itemProp = _collectionProperty.GetArrayElementAtIndex(i);
                if (itemProp.managedReferenceValue == null)
                {
                    _statesProperty?.DeleteArrayElementAtIndex(i);
                    _collectionProperty.DeleteArrayElementAtIndex(i);
                    _serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    i--;
                }
                else
                {
                    SerializedProperty stateProp = _statesProperty?.GetArrayElementAtIndex(i);
                    AddRow(itemProp, stateProp);
                }
            }
        }
        
        private void AddRow(SerializedProperty itemProp, SerializedProperty stateProp)
        {
            var row = new ManagedCollectionItem(_serializedObject, itemProp, stateProp, r =>
            {
                var index = _items.IndexOf(r);
                _items.RemoveAt(index);
                _statesProperty?.DeleteArrayElementAtIndex(index);
                _collectionProperty.DeleteArrayElementAtIndex(index);
                r.RemoveFromHierarchy();
                _serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(_target);
            });
            _itemsContainer.Add(row);
            _items.Add(row);
        }
        
        private void OnAddClick()
        {
            var type = _collectionProperty.GetPropertyType().GetElementType();
            TypeSearchWindow.Open(type, _label.text, selectedType =>
            {
                _collectionProperty.InsertArrayElementAtIndex(_collectionProperty.arraySize);
                var elementProperty = _collectionProperty.GetArrayElementAtIndex(_collectionProperty.arraySize - 1);
                elementProperty.managedReferenceValue = _factory(selectedType);

                SerializedProperty stateProperty = null;
                if (_statesProperty != null)
                {
                    _statesProperty.InsertArrayElementAtIndex(_statesProperty.arraySize);
                    stateProperty = _statesProperty.GetArrayElementAtIndex(_statesProperty.arraySize - 1);
                    stateProperty.boolValue = true;
                }
                
                _serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(_target);
                AddRow(elementProperty, stateProperty);
            });
        }
    }
}