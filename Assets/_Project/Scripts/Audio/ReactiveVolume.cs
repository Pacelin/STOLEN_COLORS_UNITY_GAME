using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class ReactiveVolume : IReactiveProperty<float>
    {
        public float Value
        {
            get => _sliderValue.Value;
            set
            {
                _sliderValue.Value = Mathf.Clamp01(value);
                _mixer.SetFloat(_volumeProperty, _sliderValue.Value.ToAudioVolume());
            }
        }
        public bool HasValue => true;

        private ReactiveProperty<float> _sliderValue;
        private readonly string _volumeProperty;
        private readonly AudioMixer _mixer;

        public ReactiveVolume(string volumeProperty, AudioMixer mixer)
        {
            _volumeProperty = volumeProperty;
            _mixer = mixer;
            _mixer.GetFloat(_volumeProperty, out var audioValue);
            _sliderValue = new(audioValue.ToAudioSlider());
        }

        public IDisposable Subscribe(IObserver<float> observer) => _sliderValue.Subscribe(observer);
    }
}