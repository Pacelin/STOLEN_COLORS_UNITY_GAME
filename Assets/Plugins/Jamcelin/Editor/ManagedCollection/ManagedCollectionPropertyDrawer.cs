using Jamcelin.Runtime.Search;
using UnityEditor;
using UnityEngine.UIElements;

namespace Jamcelin.Editor.ManagedCollection
{
    [CustomPropertyDrawer(typeof(ManagedCollectionAttribute))]
    public class ManagedCollectionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var att = attribute as ManagedCollectionAttribute;
            SerializedProperty statesProp = null;
            if (att.StatesProperty != null)
                statesProp = property.serializedObject.FindProperty(att.StatesProperty);
            
            return new ManagedCollectionField(att.Caption, property.serializedObject.targetObject,
                property.serializedObject, property, statesProp);
        }
    }
}