using UnityEngine;
using Zenject;

namespace Audio
{
    public class Ost : MonoBehaviour
    {
        [SerializeField] private EMusicKey _ost;
        [Inject] private AudioSystem _audio;
        
        private void Awake() =>
            _audio.PlayMusic(_ost, true).Forget();
    }
}