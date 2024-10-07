using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WarriorsCollection : IInitializable, IDisposable
    {
        public bool HasEnemiesFor(EBattleSide side)
        {
            if (side == EBattleSide.Ally)
                return _enemies.Count > 0;
            return _allies.Count > 0;
        }

        public IReadOnlyList<Warrior> Allies => _allies;

        private readonly WarriorsSpawner _spawner;
        private readonly List<Warrior> _enemies;
        private readonly List<Warrior> _allies;
        private readonly CompositeDisposable _disposables;
        
        public WarriorsCollection(WarriorsSpawner spawner)
        {
            _spawner = spawner;
            _enemies = new();
            _allies = new();
            _disposables = new();
        }

        public void Initialize()
        {
            _spawner.OnSpawnAlly.Subscribe(AddAlly)
                .AddTo(_disposables);
            _spawner.OnSpawnEnemy.Subscribe(AddEnemy)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public void AddEnemy(Warrior warrior)
        {
            _enemies.Add(warrior);
            warrior.OnDie
                .Subscribe(_ => _enemies.Remove(warrior))
                .AddTo(_disposables);
        }

        public void AddAlly(Warrior warrior)
        {
            _allies.Add(warrior);
            warrior.OnDie
                .Subscribe(_ => _allies.Remove(warrior))
                .AddTo(_disposables);
        }

        public Warrior GetNearestEnemyFor(Warrior warrior, out float nearestDistance)
        {
            var side = 1 - warrior.Side;
            var list = side == EBattleSide.Ally ? _allies : _enemies;
            nearestDistance = 0;
            if (list.Count == 0)
                return null;
            
            var nearest = list[0];
            nearestDistance = Vector3.Distance(warrior.Position, nearest.Position);
            foreach (var enemy in list)
            {
                var dist = Vector3.Distance(enemy.Position, warrior.Position);
                if (dist < nearestDistance)
                {
                    nearestDistance = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }
    }
}