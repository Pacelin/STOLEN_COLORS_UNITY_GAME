using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.Map.Enemies;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

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

        private Dictionary<string, AudioClip> _streamingClips = new();
        private CancellationTokenSource _cts;
        private Volumes _volumes;

        public void PlaySound(ESoundKey sound) =>
            StartCoroutine(PlaySoundInternal(sound));

        public void PlayMusic(EMusicKey key, bool loop) =>
            StartCoroutine(PlayMusicInternal(key, loop));
        
        public void Dispose() =>
            DOTween.Kill(this);
        
        private IEnumerator PlaySoundInternal(ESoundKey sound)
        {
            if (!_sounds.ContainsKey(sound) || _sounds[sound] == null || _sounds[sound].Length == 0)
            {
                Debug.LogWarning("Sound not found: " + sound);
                yield break;
            }
            var soundEntry = _sounds[sound].GetRandom();
            AudioClip clip;

            if (soundEntry.IsStreaming)
            {
                if (string.IsNullOrWhiteSpace(soundEntry.AudioPath))
                {
                    Debug.LogWarning("Incorrect path of sound: " + sound);
                    yield break;
                }

                if (!_streamingClips.ContainsKey(soundEntry.AudioPath))
                    yield return LoadStreaming(soundEntry.AudioPath);
                if (!_streamingClips.ContainsKey(soundEntry.AudioPath))
                {
                    Debug.LogWarning("Incorrect path of sound: " + sound);
                    yield break;
                }

                clip = _streamingClips[soundEntry.AudioPath];
            }
            else
            {
                if (soundEntry.Clip == null)
                {
                    Debug.LogWarning("Audio not assigned: " + sound);
                    yield break;
                }

                clip = soundEntry.Clip;
            }
            
            var source = _audioSources[soundEntry.Category];
            source.PlayOneShot(clip, soundEntry.VolumeMultiplier);
        }
        
        private IEnumerator PlayMusicInternal(EMusicKey music, bool loop)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            
            if (!_music.ContainsKey(music) || _music[music] == null)
            {
                Debug.LogWarning("Music not found: " + music);
                yield break;
            }
            var musicEntry = _music[music];
            AudioClip clip;

            if (musicEntry.IsStreaming)
            {
                if (string.IsNullOrWhiteSpace(musicEntry.AudioPath))
                {
                    Debug.LogWarning("Incorrect path of music: " + music);
                    yield break;
                }

                if (!_streamingClips.ContainsKey(musicEntry.AudioPath))
                    yield return LoadStreaming(musicEntry.AudioPath);
                if (!_streamingClips.ContainsKey(musicEntry.AudioPath))
                {
                    Debug.LogWarning("Incorrect path of music: " + music);
                    yield break;
                }

                clip = _streamingClips[musicEntry.AudioPath];
            }
            else
            {
                if (musicEntry.Clip == null)
                {
                    Debug.LogWarning("Audio not assigned: " + music);
                    yield break;
                }

                clip = musicEntry.Clip;
            }
            if (token.IsCancellationRequested)
                yield break;
            
            var source = _audioSources[musicEntry.Category];
            if (source.isPlaying)
            {
                yield return FadeOffSource(source, token);
                source.Stop();
                if (token.IsCancellationRequested)
                    yield break;
            }
            source.volume = 0;
            source.clip = clip;
            source.loop = loop;
            source.Play();
            yield return FadeOnSource(source, musicEntry.VolumeMultiplier, token);
        }
        
        private IEnumerator FadeOffSource(AudioSource audioSource, CancellationToken token)
        {
            yield return audioSource.DOFade(0, _fadeOffDuration).SetEase(_fadeOffEase).SetTarget(this)
                .WithCancellation(token).ToCoroutine();
        }
        private IEnumerator FadeOnSource(AudioSource audioSource, float volume, CancellationToken token)
        {
            yield return audioSource.DOFade(volume, _fadeOnDuration).SetEase(_fadeOnEase).SetTarget(this)
                .WithCancellation(token).ToCoroutine();
        }

        private IEnumerator LoadStreaming(string key)
        {
            var path = Path.Combine(Application.streamingAssetsPath, key);
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    yield break;
                }

                _streamingClips[key] = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }
}