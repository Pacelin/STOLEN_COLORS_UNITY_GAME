using System;
using System.Diagnostics;

namespace Jamcelin.Runtime.Search
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeSearchAttribute : Attribute
    {
        public string Path { get; }
        public string Name { get; }

        public TypeSearchAttribute(string path, string name = null)
        {
            Path = path;
            Name = name;
        }
    }
}