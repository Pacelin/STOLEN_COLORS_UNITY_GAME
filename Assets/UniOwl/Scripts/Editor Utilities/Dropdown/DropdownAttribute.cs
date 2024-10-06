using System;
using System.Diagnostics;
using UnityEngine;

namespace UniOwl
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DropdownAttribute : PropertyAttribute
    {
        public bool IncludeSelf { get; }
        public bool IncludeNone { get; }

        public DropdownAttribute(bool includeSelf = false, bool includeNone = false)
        {
            IncludeSelf = includeSelf;
            IncludeNone = includeNone;
        }
    }
}
