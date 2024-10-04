using System.Diagnostics;
using UnityEngine;

namespace Jamcelin.Runtime.Search
{
    [Conditional("UNITY_EDITOR")]
    public class ManagedCollectionAttribute : PropertyAttribute
    {
        public string Caption { get; }
        public string StatesProperty { get; }
        
        public ManagedCollectionAttribute(string caption, string statesProperty = null)
        {
            Caption = caption;
            StatesProperty = statesProperty;
        }
    }
}