using System;
using Gameplay.Map.Enemies;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WarriorsSpawner : IDisposable
    {
        public IObservable<Warrior> OnSpawnAlly => _onSpawnAlly;
        public IObservable<Warrior> OnSpawnEnemy => _onSpawnEnemy;

        public Castle EnemiesCastle => _castles.GetCastle(EBattleSide.Enemy);
        public Castle AlliesCastle => _castles.GetCastle(EBattleSide.Ally);

        private readonly DiContainer _container;
        private readonly WarriorsConfig _warriorsConfig;
        private readonly CastlesCollection _castles;

        private ReactiveCommand<Warrior> _onSpawnAlly;
        private ReactiveCommand<Warrior> _onSpawnEnemy;
        private CompositeDisposable _disposables;

        public WarriorsSpawner(DiContainer container, WarriorsConfig warriorsConfig, CastlesCollection castles)
        {
            _container = container;
            _warriorsConfig = warriorsConfig;
            _castles = castles;
            _onSpawnAlly = new();
            _onSpawnEnemy = new();
        }

        public void SpawnEnemiesWave()
        {
            if (!EnemiesCastle)
                return;
            _disposables?.Dispose();
            _disposables = new();
            var wave = EnemiesCastle.Enemies.FirstWaveVariations.GetRandom();
            SpawnWave(wave);
            Observable.Interval(TimeSpan.FromSeconds(EnemiesCastle.Enemies.WaveCooldown))
                .Subscribe(_ => SpawnWave(EnemiesCastle.Enemies.ReinforcementWaves.GetRandom()))
                .AddTo(_disposables);
        }

        public void SpawnAlliesWave(WarriorsWave wave)
        {
            if (!AlliesCastle)
                return;
            foreach (var composition in wave.Composition)
                for (int i = 0; i < composition.Count; i++)
                    SpawnAlly(composition.Class, composition.Modifiers, false);
            AlliesCastle.ReleaseIfNeed();
        }

        public void StopWave() => Dispose();

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
        
        public void SpawnEnemy(EWarriorClass @class, UnitModifiers modifiers = default, bool release = true)
        {
            var castle = _castles.GetCastle(EBattleSide.Enemy);
            var position = castle.GetWarriorSpawnPosition(EBattleSide.Enemy, @class);
            var enemy = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetEnemy(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            if (release)
                castle.AddUnit(enemy);
            else
                castle.AddUnitWithoutRelease(enemy);
            _onSpawnEnemy.Execute(enemy);
        }

        public void SpawnAlly(EWarriorClass @class, UnitModifiers modifiers = default, bool release = true)
        {
            var castle = _castles.GetCastle(EBattleSide.Ally);
            var position = castle.GetWarriorSpawnPosition(EBattleSide.Ally, @class);
            var ally = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetAlly(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            if (release)
                castle.AddUnit(ally);
            else
                castle.AddUnitWithoutRelease(ally);
            _onSpawnAlly.Execute(ally);
        }

        public void SpawnWave(WarriorsWave wave)
        {
            foreach (var composition in wave.Composition)
                for (int i = 0; i < composition.Count; i++)
                    SpawnEnemy(composition.Class, composition.Modifiers, false);
            EnemiesCastle.ReleaseIfNeed();
        }
    }
}