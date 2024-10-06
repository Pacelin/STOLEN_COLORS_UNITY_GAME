using System.Collections.Generic;
using Audio.Gameplay.PointsGrid;
using Gameplay.Map.Enemies;
using Gameplay.Map.Spawn;

namespace Gameplay.Map.Allies
{
    public class AlliesSpawner
    {
        private readonly GridPanel _gridPanel;
        private readonly WarriorsSpawner _spawner;
        private readonly WarriorsCollection _warriors;
        private readonly SpawnModifiers _constantModifiers;
        private readonly SpawnModifiers _momentModifiers;
        
        public AlliesSpawner(GridPanel gridPanel, WarriorsSpawner warriorsSpawner, WarriorsCollection warriors)
        {
            _gridPanel = gridPanel;
            _spawner = warriorsSpawner;
            _warriors = warriors;
            _constantModifiers = new();
            _momentModifiers = new();
        }

        public void Spawn()
        {
            _momentModifiers.DamageMultiplier = 1 + _constantModifiers.DamageMultiplier;
            _momentModifiers.HealthMultiplier = 1 + _constantModifiers.HealthMultiplier;
            _momentModifiers.WalkSpeed = 0 + _constantModifiers.WalkSpeed;
            _momentModifiers.MagesAttackRange = 0 + _constantModifiers.MagesAttackRange;
            _momentModifiers.AttackSpeedMultiplier = 1 + _constantModifiers.AttackSpeedMultiplier;
            _momentModifiers.MagesCount = 0;
            _momentModifiers.SoldiersCount = 0;
            _momentModifiers.TanksCount = 0;
            
            foreach (var point in _gridPanel.Grid)
                for (int i = 0; i < point.ActivationsCount; i++)
                    point.Model.Action.ApplyAction(_momentModifiers, _constantModifiers, _warriors);

            var wave = new WarriorsWave();
            var composition = new List<WarriorComposition>();
            if (_momentModifiers.TanksCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class =  EWarriorClass.Tank,
                    Count = _momentModifiers.TanksCount,
                    Modifiers = _momentModifiers
                });
            if (_momentModifiers.SoldiersCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class = EWarriorClass.Soldier,
                    Count = _momentModifiers.SoldiersCount,
                    Modifiers = _momentModifiers
                });
            if (_momentModifiers.MagesCount > 0)
                composition.Add(new WarriorComposition()
                {
                    Class = EWarriorClass.Mage,
                    Count = _momentModifiers.MagesCount,
                    Modifiers = _momentModifiers
                });
            if (composition.Count > 0)
            {
                wave.Composition = composition.ToArray();
                _spawner.SpawnAlliesWave(wave);
            }
        }
    }
}