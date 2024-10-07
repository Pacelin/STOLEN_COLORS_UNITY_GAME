using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WaveState : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _cooldown;
        [SerializeField] private TMP_Text _timeText;

        [Inject] private WaveManager _manager;

        private float _time;
        private IDisposable _timerDisposable;
        private IDisposable _cooldownDisposable;

        private void OnEnable()
        {
            _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(_delay))
                .Subscribe(_ => StartTime());
            _timeText.text = "";
        }

        private void OnDisable()
        {
            _timerDisposable?.Dispose();
            _cooldownDisposable?.Dispose();
        }

        private void StartTime()
        {
            _time = _cooldown;
            _timerDisposable?.Dispose();
            _timeText.text = _time.ToString("0");
            _cooldownDisposable = Observable.Interval(TimeSpan.FromSeconds(1))
                .Subscribe(_ =>
                {
                    _time--;
                    _timeText.text = _time.ToString("0");
                });
            _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(_cooldown))
                .Subscribe(_ => StartWave());
        }

        private void StartWave()
        {
            _timeText.text = "";
            _cooldownDisposable?.Dispose();
            _timerDisposable?.Dispose();
            _manager.StartWave();
            
            _timerDisposable = _manager.WaveIsInProgress.Where(w => !w)
                .Subscribe(_ => StartTime());
        }
    }
}