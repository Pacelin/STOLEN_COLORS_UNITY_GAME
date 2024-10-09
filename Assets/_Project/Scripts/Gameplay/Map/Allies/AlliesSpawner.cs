using System;
using System.Collections.Generic;
using Audio.Gameplay.PointsGrid;
using Gameplay.Map.Enemies;
using Gameplay.Map.Spawn;
using UniRx;

namespace Gameplay.Map.Allies
{
    public class AlliesSpawner
    {
        public class SpawnCount
        {
            public int MagesCount;
            public int WarriorsCount;
            public int TanksCount;
        }
        public IObservable<SpawnModifiers> OnChangeModifiers => _onChangeModifiers;
        public SpawnModifiers ConstantModifiers => _constantModifiers;
        
        private readonly GridPanel _gridPanel;
        private readonly WarriorsSpawner _spawner;
        private readonly WarriorsCollection _warriors;
        private readonly SpawnModifiers _constantModifiers;
        private readonly ReactiveCommand<SpawnModifiers> _onChangeModifiers;

        public AlliesSpawner(GridPanel gridPanel, WarriorsSpawner warriorsSpawner, WarriorsCollection warriors)
        {
            _gridPanel = gridPanel;
            _spawner = warriorsSpawner;
            _warriors = warriors;
            _constantModifiers = new();
            _constantModifiers.DamageMultiplier = 1;
            _constantModifiers.HealthMultiplier = 1;
            _constantModifiers.AttackSpeedMultiplier = 1;
            _onChangeModifiers = new();
        }

        public void Spawn()
        {
            var count = new SpawnCount();
            foreach (var point in _gridPanel.Grid)
                for (int i = 0; i < point.ActivationsCount; i++)
                    point.Model.Action.ApplyAction(count, _constantModifiers, _warriors);

            foreach (var ally in _warriors.Allies)
                ally.UpdateModifiers(_constantModifiers);
            
            var wave = new WarriorsWave();
            var composition = new List<WarriorComposition>();
            if (count.TanksCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class =  EWarriorClass.Tank,
                    Count = count.TanksCount,
                    Modifiers = _constantModifiers
                });
            if (count.WarriorsCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class = EWarriorClass.Soldier,
                    Count = count.WarriorsCount,
                    Modifiers = _constantModifiers
                });
            if (count.MagesCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class = EWarriorClass.Mage,
                    Count = count.MagesCount,
                    Modifiers = _constantModifiers
                });
            if (composition.Count > 0)
            {
                wave.Composition = composition.ToArray();
                _spawner.SpawnAlliesWave(wave);
            }

            _onChangeModifiers.Execute(_constantModifiers);
        }
    }
}