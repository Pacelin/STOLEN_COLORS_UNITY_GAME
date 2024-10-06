using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UniOwl.Editor
{
    public static class EditorUtils
    {
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty parent, string[] excludeFields = null)
        {
            SerializedProperty it = parent.Copy();
            SerializedProperty itEnd = parent.Copy();
                
            if (!itEnd.NextVisible(false))
                itEnd = null;

            if (!it.NextVisible(true))
                it = null;
            
            // If has no children
            if (it == null || itEnd != null && it.propertyPath == itEnd.propertyPath)
                yield break;
            
            do
            {
                if (SerializedProperty.EqualContents(it, itEnd))
                    yield break;
                if (excludeFields != null && excludeFields.Contains(it.name))
                    continue;

                yield return it;

            } while (it.NextVisible(false));
        }
        
        public static IEnumerable<SerializedProperty> GetChildren(this SerializedObject parent, string[] excludeFields = null)
        {
            SerializedProperty it = parent.GetIterator();
            it.NextVisible(true);
            
            do
            {
                if (excludeFields != null && excludeFields.Contains(it.name))
                    continue;

                yield return it;
            } while (it.NextVisible(false));
        }

        public static bool HasAttribute<TAttribute>(this SerializedProperty property) where TAttribute : Attribute
        {
            TAttribute attr = property.GetAttribute<TAttribute>();

            return attr != null;
        }
        
        public static bool TryGetPropertyWithAttribute<TAttribute>(this SerializedObject obj, out SerializedProperty property, out TAttribute attribute, string[] excludeFields = null, bool inherit = true) where TAttribute : Attribute
        {
            foreach (SerializedProperty child in obj.GetChildren(excludeFields))
            {
                TAttribute attr = child.GetAttribute<TAttribute>();
                if (attr == null) continue;

                property = child;
                attribute = attr;
                return true;
            }

            property = null;
            attribute = null;
            return false;
        }

        [NotNull]
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this SerializedProperty property) where TAttribute : Attribute
        {
            if (property == null) return Enumerable.Empty<TAttribute>();

            Type targetType = property.serializedObject.targetObject.GetType();

            var fi = property.GetPropertyFieldInfo();
            return fi!.GetCustomAttributes<TAttribute>();
        }

        public static TAttribute GetAttribute<TAttribute>(this SerializedProperty property) where TAttribute : Attribute
        {
            IEnumerable<TAttribute> attrs = GetAttributes<TAttribute>(property);
            return attrs.FirstOrDefault();
        }
    }
}