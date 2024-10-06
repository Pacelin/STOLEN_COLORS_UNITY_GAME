using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Audio
{
    public class AudioSystemInitializer : MonoBehaviour
    {
        [Inject]
        private AudioSystem _audioSystem;
        
        [SerializeField]
        private Slider _sfxSlider, _musicSlider;

        private void OnEnable()
        {
            _sfxSlider.SetValueWithoutNotify(_audioSystem.Volume[EAudioCategory.Sound].Value);
            _musicSlider.SetValueWithoutNotify(_audioSystem.Volume[EAudioCategory.Music].Value);
            
            _sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicValueChanged);
        }

        private void OnDestroy()
        {
            _sfxSlider.onValueChanged.RemoveListener(OnSfxValueChanged);
            _musicSlider.onValueChanged.RemoveListener(OnMusicValueChanged);
        }

        private void OnSfxValueChanged(float value)
        {
            _audioSystem.Volume[EAudioCategory.Sound].Value = value;
        }
        
        private void OnMusicValueChanged(float value)
        {
            _audioSystem.Volume[EAudioCategory.Music].Value = value;
        }
    }
}
