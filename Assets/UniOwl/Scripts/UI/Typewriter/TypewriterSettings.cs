using UnityEngine;

namespace UniOwl.UI
{
    [CreateAssetMenu(fileName = "SO_TypewriterSettings", menuName = "Game/UI/Typewriter")]
    public class TypewriterSettings : ScriptableObject
    {
        [SerializeField]
        private float _typeSpeed;

        public float TypeSpeed => _typeSpeed;
    }
}