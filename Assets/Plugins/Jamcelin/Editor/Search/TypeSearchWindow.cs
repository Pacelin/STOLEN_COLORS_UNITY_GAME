using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Jamcelin.Editor.Search
{
    public static class TypeSearchWindow
    {
        public static void Open(Type type, string windowName, Action<Type> callback)
        {
            var point = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            var provider = ScriptableObject.CreateInstance<TypeSearch>();
            provider.SetWindowName(windowName);
            provider.SetType(type);
            provider.RegisterCallback(callback);
            SearchWindow.Open(new SearchWindowContext(point), provider);
        }
    }
}