using System;
using Jamcelin.Editor.Search;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jamcelin.Editor.ManagedCollection
{
    public class ManagedCollectionItem : VisualElement
    {
        public ManagedCollectionItem(SerializedObject root, 
            SerializedProperty itemProp, SerializedProperty stateProp, Action<ManagedCollectionItem> removeCmd)
        {
            style.backgroundColor = new Color(1, 1, 1, 0.05f);
            style.marginTop = 8;
            style.paddingBottom = 5;
            style.paddingTop = 5;
            style.paddingLeft = 5;
            style.paddingRight = 5;
            style.borderBottomLeftRadius = 5;
            style.borderBottomRightRadius = 5;
            style.borderTopLeftRadius = 5;
            style.borderTopRightRadius = 5;
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;

            if (stateProp != null)
            {
                var toggle = new Toggle();
                toggle.value = stateProp.boolValue;
                toggle.style.marginBottom = 0;
                toggle.RegisterValueChangedCallback(t =>
                {
                    stateProp.boolValue = t.newValue;
                    root.ApplyModifiedProperties();
                });
                Add(toggle);
            }

            var type = itemProp.managedReferenceValue.GetType();
            var icon = new Image();
            icon.style.width = 18;
            icon.style.height = 18;
            icon.style.marginLeft = 10;
            icon.style.marginRight = 5;
            icon.scaleMode = ScaleMode.ScaleToFit;
            icon.image = type.GetIcon();
            Add(icon);

            var label = new Label(type.SplittedName());
            label.style.flexGrow = 1;
            Add(label);

            var removeButton = new Button(() => removeCmd(this));
            removeButton.text = "-";
            Add(removeButton);
        }
    }
}