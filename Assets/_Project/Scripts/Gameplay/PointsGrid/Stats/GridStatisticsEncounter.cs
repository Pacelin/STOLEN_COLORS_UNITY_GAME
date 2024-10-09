using System.Collections.Generic;
using Gameplay.Map;
using Gameplay.Map.Spawn;
using UniRx;
using UnityEngine;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class GridStatisticsEncounter : MonoBehaviour
    {
        [SerializeField] private GridStatisticsContainer _container;
        [Inject] private GridPanel _grid;
        [Inject] private WaveManager _wave;

        private GridStatisticsUnit _waveActivationsCollect;
        private GridStatisticsUnit _waveCountCollect;
        
        private CompositeDisposable _disposables;

        private void OnEnable()
        {
            _disposables = new();
            _wave.WaveIsInProgress.SkipLatestValueOnSubscribe()
                .Subscribe(b =>
            {
                if (b) StopCollectWave();
                else StartCollectWave();
            }).AddTo(_disposables);
            StartCollectWave();
            _grid.OnApply.Subscribe(CollectGrid);
        }

        private void OnDisable()
        {
            _disposables?.Dispose();
        }

        private void StartCollectWave()
        {
            _waveActivationsCollect = new();
            _waveCountCollect = new();
        }

        private void StopCollectWave()
        {
            _container.PerWaveCount.Average(_waveCountCollect);
            _container.PerWaveActivationsCount.Average(_waveActivationsCollect);
        }

        private void CollectGrid(IEnumerable<GridPoint> points)
        {
            var gridCount = new GridStatisticsUnit();
            var gridActivations = new GridStatisticsUnit();
            foreach (var point in points)
                CollectPoint(point, gridCount, gridActivations);
            _waveCountCollect.Add(gridCount);
            _waveActivationsCollect.Add(gridActivations);
            _container.OverallCount.Add(gridCount);
            _container.OverallActivationsCount.Add(gridActivations);
            _container.PerGridCount.Average(gridCount);
            _container.PerGridActivationsCount.Average(gridActivations);
        }

        private void CollectPoint(GridPoint point, GridStatisticsUnit count, GridStatisticsUnit activations)
        {
            if (point.IsEmptyPoint)
                count.EmptyPoints++;
            else if (point.Model.Action is EmptyAction)
                count.Locks++;
            else if (point.Model.Action is HealAction)
            {
                count.Heal++;
                activations.Heal += point.ActivationsCount;
            }
            else if (point.Model.Action is IncreaseAttackSpeedAction)
            {
                count.AttackSpeed++;
                activations.AttackSpeed += point.ActivationsCount;
            }
            else if (point.Model.Action is IncreaseDamageAction)
            {
                count.AttackDamage++;
                activations.AttackDamage += point.ActivationsCount;
            }
            else if (point.Model.Action is IncreaseHealthAction)
            {
                count.Health++;
                activations.Health += point.ActivationsCount;
            }
            else if (point.Model.Action is IncreaseMagesAttackRangeAction)
            {
                count.Range++;
                activations.Range += point.ActivationsCount;
            }
            else if (point.Model.Action is IncreaseWalkSpeedAction)
            {
                count.MoveSpeed++;
                activations.MoveSpeed += point.ActivationsCount;
            }
            else if (point.Model.Action is SpawnWarriorAction spawnAction)
            {
                if (spawnAction.Class == EWarriorClass.Mage)
                {
                    count.MagesSpawn++;
                    activations.MagesSpawn += point.ActivationsCount;
                }
                else if (spawnAction.Class == EWarriorClass.Soldier)
                {
                    count.WarriorsSpawn++;
                    activations.WarriorsSpawn += point.ActivationsCount;
                }
                else if (spawnAction.Class == EWarriorClass.Tank)
                {
                    count.TanksSpawn++;
                    activations.TanksSpawn += point.ActivationsCount;
                }
            }
        }
    }
}