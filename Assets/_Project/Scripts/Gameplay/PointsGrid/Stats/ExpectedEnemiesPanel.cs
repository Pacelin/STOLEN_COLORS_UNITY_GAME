using DG.Tweening;
using Gameplay.Map;
using Gameplay.Map.Enemies;
using Gameplay.Map.Spawn;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class ExpectedEnemiesPanel : MonoBehaviour
    {
        [SerializeField] private float _closedX;
        [SerializeField] private float _openedX;
        [SerializeField] private float _duration;
        [SerializeField] private float _delay;
        [Space] 
        [SerializeField] private GameObject _defaultState;
        [SerializeField] private GameObject _bossState;
        [SerializeField] private TMP_Text _magesCount;
        [SerializeField] private TMP_Text _warriorsCount;
        [SerializeField] private TMP_Text _tanksCount;
        
        [Inject] private WaveManager _wave;
        [Inject] private WarriorsSpawner _spawner;
        
        private CompositeDisposable _disposables;
        private Tween _tween;
        
        private void OnEnable()
        {
            _disposables = new();
            var transform1 = transform;
            var p = transform1.localPosition;
            p.x = _closedX;
            transform1.localPosition = p;
            _tween = DOTween.Sequence()
                .AppendInterval(_delay)
                .Append(transform.DOLocalMoveX(_openedX, _duration));
            _wave.WaveIsInProgress.Skip(1).Subscribe(b =>
            {
                if (b)
                    _tween = transform.DOLocalMoveX(_closedX, _duration);
                else
                    _tween = transform.DOLocalMoveX(_openedX, _duration);
            }).AddTo(_disposables);
            _spawner.PreparedEnemiesWave.Subscribe(wave => UpdateExpectedEnemies(wave, _spawner.HasPreparedEnemiesWave))
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _disposables.Dispose();
        }

        private void UpdateExpectedEnemies(WarriorsWave wave, bool hasWave)
        {
            if (!hasWave)
            {
                _defaultState.SetActive(false);
                _bossState.SetActive(true);
            }
            else
            {
                _defaultState.SetActive(true);
                _bossState.SetActive(false);
                int magesCount = 0;
                int warriorsCount = 0;
                int tanksCount = 0;
                foreach (var composition in wave.Composition)
                {
                    if (composition.Class == EWarriorClass.Mage)
                        magesCount += composition.Count;
                    else if (composition.Class == EWarriorClass.Soldier)
                        warriorsCount += composition.Count;
                    else if (composition.Class == EWarriorClass.Tank)
                        tanksCount += composition.Count;
                }

                _magesCount.text = "x" + magesCount;
                _warriorsCount.text = "x" + warriorsCount;
                _tanksCount.text = "x" + tanksCount;
            }
        }
    }
}