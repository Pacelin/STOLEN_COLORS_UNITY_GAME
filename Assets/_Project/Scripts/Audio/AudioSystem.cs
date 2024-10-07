using System;
using System.Collections.Generic;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Map.Enemies;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    public class AudioSystem : MonoBehaviour, IDisposable
    {
        public class Volumes
        {
            public ReactiveVolume Master => _masterVolume;
        
            private readonly ReactiveVolume _masterVolume;
            private readonly Dictionary<EAudioCategory, ReactiveVolume> _volumes;
        
            public Volumes(AudioMixer mixer, string masterVolumeProperty, Dictionary<EAudioCategory, string> keyProperties)
            {
                _masterVolume = new ReactiveVolume(masterVolumeProperty, mixer);
                _volumes = new Dictionary<EAudioCategory, ReactiveVolume>();
                var alreadyAdded = new Dictionary<string, ReactiveVolume>();
                foreach (var keyProperty in keyProperties)
                {
                    if (alreadyAdded.ContainsKey(keyProperty.Value))
                    {
                        _volumes.Add(keyProperty.Key, alreadyAdded[keyProperty.Value]);
                    }
                    else
                    {
                        var newVolume = new ReactiveVolume(keyProperty.Value, mixer);
                        _volumes.Add(keyProperty.Key, newVolume);
                        alreadyAdded.Add(keyProperty.Value, newVolume);
                    }
                }
            }

            public ReactiveVolume this[EAudioCategory category] => _volumes[category];
        }

        public Volumes Volume
        {
            get
            {
                if (_volumes == null)
                    _volumes = new Volumes(_mixer, _masterVolumeProperty, _volumeProperties);
                return _volumes;
            }
        }
        
        [SerializeField] private string _masterVolumeProperty;
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private SerializedDictionary<EAudioCategory, AudioSource> _audioSources = new();
        [SerializeField] private SerializedDictionary<EAudioCategory, string> _volumeProperties = new();
        [SerializeField] private SerializedDictionary<ESoundKey, AudioEntry[]> _sounds = new();
        [SerializeField] private SerializedDictionary<EMusicKey, AudioEntry> _music = new();

        [Header("Fade")] 
        [SerializeField] private float _fadeOffDuration;
        [SerializeField] private Ease _fadeOffEase;
        [Space]
        [SerializeField] private float _fadeOnDuration;
        [SerializeField] private Ease _fadeOnEase;

        private CancellationTokenSource _cts;
        private Volumes _volumes;

        public void PlaySound(ESoundKey sound)
        {
            var soundEntry = _sounds[sound].GetRandom();
            var source = _audioSources[soundEntry.Category];
            source.PlayOneShot(soundEntry.Clip, soundEntry.VolumeMultiplier);
        }

        public async UniTaskVoid PlayMusic(EMusicKey key, bool loop)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            var musicEntry = _music[key];
            var source = _audioSources[musicEntry.Category];
            if (source.isPlaying)
            {
                await FadeOffSource(source, token);
                source.Stop();
                if (token.IsCancellationRequested)
                    return;
            }

            source.volume = 0;
            source.clip = musicEntry.Clip;
            source.loop = loop;
            source.Play();
            await FadeOnSource(source, musicEntry.VolumeMultiplier, token);
        }
        
        private async UniTask FadeOffSource(AudioSource audioSource, CancellationToken token) =>
            await audioSource.DOFade(0, _fadeOffDuration).SetEase(_fadeOffEase).SetTarget(this)
                .WithCancellation(token);
        private async UniTask FadeOnSource(AudioSource audioSource, float volume, CancellationToken token) =>
            await audioSource.DOFade(volume, _fadeOnDuration).SetEase(_fadeOnEase).SetTarget(this)
                .WithCancellation(token);

        public void Dispose() =>
            DOTween.Kill(this);
    }
}