using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using ModestTree;
using UnityEditor;
using UnityEngine;

namespace Jamcelin.Editor.Search
{
    public static class TypeExtensions
    {
        private static Texture2D _defaultTexture;
        private static Dictionary<string, Texture2D> _texturesCache;

        public static Texture2D GetIcon(this Type type)
        {
            if (_texturesCache == null)
                _texturesCache = new();
            
            var typeIconAttr = type.TryGetAttribute<IconAttribute>();
            if (typeIconAttr != null)
            {
                if (!_texturesCache.ContainsKey(typeIconAttr.path))
                {
                    var iconAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(typeIconAttr.path);
                    var texture = new Texture2D(128, 128);
                    EditorUtility.CopySerialized(iconAsset, texture);
                    _texturesCache.Add(typeIconAttr.path, texture);
                }
                return _texturesCache[typeIconAttr.path];
            }

            return GetDefaultTypeIcon();
        }
        
        public static Texture2D GetDefaultTypeIcon()
        {
            if (_defaultTexture == null)
            {
                _defaultTexture = new Texture2D(1, 1);
                _defaultTexture.SetPixel(0, 0, Color.clear);
                _defaultTexture.Apply();
            }
            return _defaultTexture;
        }
        
        public static string SplittedName(this Type type)
        {
            var str = type.Name;
            var builder = new StringBuilder();
            builder.Append(str[0]);
            for (int i = 1; i < str.Length; i++)
            {
                var isNeedByDigit = char.IsDigit(str[i]) && !char.IsDigit(str[i - 1]);
                var isNeedByUpper = char.IsUpper(str[i]) && !char.IsUpper(str[i - 1]);

                if (isNeedByDigit || isNeedByUpper)
                    builder.Append(' ');
                builder.Append(str[i]);
            }

            return builder.ToString();
        }

        public static Type GetPropertyType(this SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            var fieldInfo = parentType.GetField(property.propertyPath, 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return fieldInfo?.FieldType;
        }
    }
}