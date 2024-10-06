using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniOwl.Editor
{
    public static class TypeUtils
    {
        private const string NoneChoiceName = "None";
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static IEnumerable<(string name, Type type)> GetDropdownChoices(Type baseType, bool includeSelf, bool includeNone)
        {
            // Include None first
            if (includeNone)
                yield return (NoneChoiceName, null);

            var derivedTypes = TypeCache.GetTypesDerivedFrom(baseType);

            var nonOrderedTypes =
                derivedTypes.
                    Where(type =>
                    {
                        // Disallow abstracts
                        if (type.IsAbstract)
                            return false;

                        return true;
                    }).
                    Select(type => (GetDisplayName(type), type));

            // Add Self
            if (includeSelf)
                nonOrderedTypes = nonOrderedTypes.Append((GetDisplayName(baseType), baseType));

            var orderedTypes = nonOrderedTypes.OrderBy(tuple => tuple.Item1);

            foreach ((string name, Type type) pair in orderedTypes)
            {
                yield return pair;
            }
        }

        private static string GetDisplayName(Type type)
        {
            return type.GetCustomAttribute<DropdownDisplayAttribute>()?.DisplayName ?? type.FullName;
        }
        
        private static void GetFieldData(object target, string fieldName, out Type type, out object value, ref FieldInfo fi)
        {
            if (fieldName.StartsWith("data["))
            {
                int indexStart = fieldName.IndexOf('[');
                string indexString = fieldName.Substring(indexStart + 1, fieldName.IndexOf(']') - indexStart - 1);
                int arrayIndex = int.Parse(indexString);

                var array = (Array)target;
                
                value = array.GetValue(arrayIndex);
                type = value?.GetType() ?? array.GetType().GetElementType();
            }
            else
            {
                Type currentType = target.GetType();
                fi = currentType.GetField(fieldName, Flags);
                    
                if (fi == null)
                    throw new Exception($"Field '{fieldName}' not found in type '{target.GetType()}'.");

                value = fi.GetValue(target);
                type = value?.GetType() ?? fi.FieldType;
            }
        }

        public static Type GetPropertyType(this SerializedProperty property)
        {
            object currentObject = property.serializedObject.targetObject;
            Type currentType = currentObject.GetType();
            FieldInfo fi = null;
            
            string[] pathParts = property.propertyPath.Replace("Array.", string.Empty).Split(".");

            foreach (string part in pathParts)
                GetFieldData(currentObject, part, out currentType, out currentObject, ref fi);

            return currentType;
        }

        public static object GetPropertyValue(this SerializedProperty property)
        {
            object currentObject = property.serializedObject.targetObject;
            FieldInfo fi = null;
            
            string[] pathParts = property.propertyPath.Replace("Array.", string.Empty).Split(".");

            foreach (string part in pathParts)
                GetFieldData(currentObject, part, out _, out currentObject, ref fi);

            return currentObject;
        }

        public static void SetPropertyValue(this SerializedProperty property, object value)
        {
            object currentObject = property.serializedObject.targetObject;
            Type currentType = currentObject.GetType();
            FieldInfo currentField = null;
            
            string[] pathParts = property.propertyPath.Replace("Array.", string.Empty).Split(".");

            for (int i = 0; i < pathParts.Length - 1; i++)
                GetFieldData(currentObject, pathParts[i], out currentType, out currentObject, ref currentField);

            string lastPart = pathParts[^1]; 
            
            if (lastPart.StartsWith("data["))
            {
                int indexStart = lastPart.IndexOf('[');
                string indexString = lastPart.Substring(indexStart + 1, lastPart.IndexOf(']') - indexStart - 1);
                int arrayIndex = int.Parse(indexString);

                var array = (Array)currentObject;
                array.SetValue(value, arrayIndex);
            }
            else
            {
                currentField = currentType.GetField(lastPart, Flags);
                
                if (currentField == null)
                    throw new Exception($"Field '{lastPart}' not found in type '{currentObject.GetType()}'.");

                currentField.SetValue(currentObject, value);
            }
        }

        public static FieldInfo GetPropertyFieldInfo(this SerializedProperty property)
        {
            object currentObject = property.serializedObject.targetObject;
            FieldInfo fi = null;
            
            string[] pathParts = property.propertyPath.Replace("Array.", string.Empty).Split(".");

            foreach (string part in pathParts)
                GetFieldData(currentObject, part, out _, out currentObject, ref fi);

            return fi;
        }
    }
}
