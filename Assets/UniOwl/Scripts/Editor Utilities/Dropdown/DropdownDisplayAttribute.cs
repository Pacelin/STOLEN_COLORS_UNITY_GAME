using System;
using System.Diagnostics;

namespace UniOwl
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class DropdownDisplayAttribute : Attribute
    {
        public string DisplayName { get; }

        public DropdownDisplayAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}