using System;
using System.Collections.Generic;
using System.Linq;
using Jamcelin.Runtime.Search;
using ModestTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Jamcelin.Editor.Search
{
    public class TypeSearch : ScriptableObject, ISearchWindowProvider
    {
        private Type _type;
        private string _name;
        private Action<Type> _callback;

        public void RegisterCallback(Action<Type> callback) => _callback = callback;
        public void SetType(Type type) => _type = type;
        public void SetWindowName(string windowName) => _name = windowName;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchList = new();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent(_name), 0));
            
            var types = EvaluateTypes(_type);
            var groups = new HashSet<string>();
            foreach (var (path, type) in types)
            {
                var split = path.Split("/");
                string groupName = "";
                for (int i = 0; i < split.Length - 1; i++)
                {
                    groupName += split[i];
                    if (!groups.Contains(groupName))
                    {
                        searchList.Add(new SearchTreeGroupEntry(new GUIContent(split[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                var entry = new SearchTreeEntry(new GUIContent(split[^1], type.GetIcon()));
                entry.level = split.Length;
                entry.userData = type;
                searchList.Add(entry);
            }
            return searchList;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _callback?.Invoke((Type) searchTreeEntry.userData);
            return true;
        }


        private static IEnumerable<(string, Type)> EvaluateTypes(Type type)
        {
            var types = TypeCache.GetTypesDerivedFrom(type).Where(t => !t.IsAbstract && !t.IsInterface);
            return types
                .Select(type =>
                {
                    var attribute = type.TryGetAttribute<TypeSearchAttribute>();
                    if (attribute == null)
                        return ("Unknown/" + type.SplittedName(), type);
                    if (attribute.Name == null)
                        return (attribute.Path + "/" + type.SplittedName(), type);
                    return (attribute.Path + "/" + attribute.Name, type);
                })
                .OrderBy(tuple => tuple.Item1);
        }
    }
}