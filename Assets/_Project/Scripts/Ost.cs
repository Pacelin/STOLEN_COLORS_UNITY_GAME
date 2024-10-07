using UnityEngine;
using Zenject;

namespace Audio
{
    public class Ost : MonoBehaviour
    {
        [SerializeField] private EMusicKey _ost;
        [Inject] private AudioSystem _audio;

        private void Awake()
        {
            Time.timeScale = 1f;
            _audio.PlayMusic(_ost, true).Forget();
        }
    }
}