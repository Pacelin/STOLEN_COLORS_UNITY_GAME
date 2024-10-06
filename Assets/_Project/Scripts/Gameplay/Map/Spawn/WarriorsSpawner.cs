using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WarriorsSpawner
    {
        public IObservable<Warrior> OnSpawnAlly => _onSpawnAlly;
        public IObservable<Warrior> OnSpawnEnemy => _onSpawnEnemy;

        private readonly DiContainer _container;
        private readonly WarriorsConfig _warriorsConfig;
        private readonly CastlesCollection _castles;

        private ReactiveCommand<Warrior> _onSpawnAlly;
        private ReactiveCommand<Warrior> _onSpawnEnemy;

        public WarriorsSpawner(DiContainer container, WarriorsConfig warriorsConfig, CastlesCollection castles)
        {
            _container = container;
            _warriorsConfig = warriorsConfig;
            _castles = castles;
            _onSpawnAlly = new();
            _onSpawnEnemy = new();
        }

        public void SpawnEnemy(EWarriorClass @class, UnitModifiers modifiers = default)
        {
            var castle = _castles.GetCastle(EBattleSide.Enemy);
            var position = castle.GetWarriorSpawnPosition(EBattleSide.Enemy, @class);
            var enemy = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetEnemy(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            castle.AddUnit(enemy);
            _onSpawnEnemy.Execute(enemy);
        }

        public void SpawnAlly(EWarriorClass @class, UnitModifiers modifiers = default)
        {
            var castle = _castles.GetCastle(EBattleSide.Ally);
            var position = castle.GetWarriorSpawnPosition(EBattleSide.Ally, @class);
            var ally = _container.InstantiatePrefabForComponent<Warrior>(_warriorsConfig.GetAlly(@class),
                position, Quaternion.identity, null, new object[] { modifiers });
            castle.AddUnit(ally);
            _onSpawnAlly.Execute(ally);
        }
    }
}