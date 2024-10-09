using System;
using Gameplay.Map.Allies;
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
            var enemiesCastle = _castles.GetCurrentCastle(EBattleSide.Enemy);
            
            if (!enemiesCastle)
                return;
            _disposables?.Dispose();
            _disposables = new();
            var wave = enemiesCastle.Enemies.FirstWaveVariations.GetRandom();
            SpawnWave(enemiesCastle, wave);
            Observable.Interval(TimeSpan.FromSeconds(enemiesCastle.Enemies.WaveCooldown))
                .Subscribe(_ => SpawnWave(enemiesCastle, enemiesCastle.Enemies.ReinforcementWaves.GetRandom()))
                .AddTo(_disposables);
        }

        public void SpawnAlliesWave(WarriorsWave wave)
        {
            var alliesCastle = _castles.GetCurrentCastle(EBattleSide.Ally);

            if (!alliesCastle)
                return;
            foreach (var composition in wave.Composition)
                for (int i = 0; i < composition.Count; i++)
                    SpawnAlly(alliesCastle, composition.Class, composition.Modifiers, false);
            alliesCastle.ReleaseIfNeed();
        }

        public void StopWave() => Dispose();

        public void Dispose()
        {
            _disposables?.Dispose();
            _disposables = null;
        }
        
        private void SpawnEnemy(Castle castle, EWarriorClass @class, SpawnModifiers modifiers = null, bool release = true)
        {
            if (modifiers == null)
                modifiers = new SpawnModifiers()
                {
                    WalkSpeed = 0,
                    AttackSpeedMultiplier = 1,
                    MagesAttackRange = 0,
                    DamageMultiplier = 1,
                    HealthMultiplier = 1
                };
            var position = castle.GetWarriorSpawnPosition(EBattleSide.Enemy, @class);
            var enemy = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetEnemy(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            if (release)
                castle.AddUnit(enemy);
            else
                castle.AddUnitWithoutRelease(enemy);
            _onSpawnEnemy.Execute(enemy);
        }

        private void SpawnAlly(Castle castle, EWarriorClass @class, SpawnModifiers modifiers = null, bool release = true)
        {
            if (modifiers == null)
                modifiers = new SpawnModifiers()
                {
                    WalkSpeed = 0,
                    AttackSpeedMultiplier = 1,
                    MagesAttackRange = 0,
                    DamageMultiplier = 1,
                    HealthMultiplier = 1
                };

            var position = castle.GetWarriorSpawnPosition(EBattleSide.Ally, @class);
            var ally = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetAlly(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            if (release)
                castle.AddUnit(ally);
            else
                castle.AddUnitWithoutRelease(ally);
            _onSpawnAlly.Execute(ally);
        }

        private void SpawnWave(Castle castle, WarriorsWave wave)
        {
            foreach (var composition in wave.Composition)
                for (int i = 0; i < composition.Count; i++)
                    SpawnEnemy(castle, composition.Class, composition.Modifiers, false);
            castle.ReleaseIfNeed();
        }
    }
}