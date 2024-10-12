using System;
using Audio.Gameplay.PointsGrid;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WaveState : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;

        [Inject] private WaveManager _manager;
        [Inject] private CastlesCollection _castles;
        [Inject] private GridPanel _panel;

        private int _remaining;
        private IDisposable _counterDisposable;

        private void OnEnable()
        {
            StartTime();
            _timeText.text = "";
        }

        private void OnDisable()
        {
            _counterDisposable?.Dispose();
        }

        private void StartTime()
        {
            _counterDisposable?.Dispose();
            _manager.PrepareEnemiesWave();
            var c = _castles.GetCurrentCastle(EBattleSide.Ally);
            if (!c)
                return;
            _remaining = c.GridsCount;
            _timeText.text = _remaining.ToString();
            _counterDisposable = _panel.OnApply.Subscribe(_ =>
            {
                _remaining--;
                _timeText.text = _remaining.ToString();
                if (_remaining == 0)
                    StartWave();
            });
        }

        private void StartWave()
        {
            _timeText.text = "";
            _counterDisposable.Dispose();
            _manager.StartWave();
            _counterDisposable = _manager.WaveIsInProgress
                .Where(b => !b).Subscribe(_ => StartTime());
        }
    }
}