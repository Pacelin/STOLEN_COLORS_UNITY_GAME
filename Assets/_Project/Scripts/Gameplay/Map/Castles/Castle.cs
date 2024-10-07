using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Gameplay.Map.Enemies;
using UnityEngine;

namespace Gameplay.Map
{
    public class Castle : MonoBehaviour
    {
        public EBattleSide Owner => _owner;
        public CastleEnemies Enemies => _enemies;
        
        [SerializeField] private CastleEnemies _enemies;
        [SerializeField] private Transform _warriorsDestination;
        [SerializeField] private EBattleSide _owner;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private SerializedDictionary<EWarriorClass, Vector3[]> _unitsCells;

        private Dictionary<EWarriorClass, List<Warrior>> _snapped = new();
        private bool _isReleased;

        public void SetOwner(EBattleSide side)
        {
            _owner = side;
        }

        public Vector3 GetWarriorDestination(Warrior warrior) =>
            new Vector3(_warriorsDestination.position.x, 0, warrior.Position.z);
        public Vector3 GetWarriorSpawnPosition(EBattleSide side, EWarriorClass @class) =>
            GetPositionFor(side, @class, GetIndex(@class));
        
        public void AddUnit(Warrior unit)
        {
            if (_isReleased)
                unit.Release();
            else
            {
                unit.Snap(GetWarriorSpawnPosition(unit.Side, unit.Class));
                AddWarrior(unit);
            }
        }

        public void AddUnitWithoutRelease(Warrior unit)
        {
            unit.Snap(GetWarriorSpawnPosition(unit.Side, unit.Class));
            AddWarrior(unit);
        }

        public void ReleaseIfNeed()
        {
            if (!_isReleased)
                return;
            foreach (var snap in _snapped)
            {
                foreach (var warrior in snap.Value)
                    warrior.Release();
                snap.Value.Clear();
            }
        }
        
        public void SnapUnits() =>
            _isReleased = false;

        public void ReleaseUnits()
        {
            foreach (var snap in _snapped)
            {
                foreach (var warrior in snap.Value)
                    warrior.Release();
                snap.Value.Clear();
            }
            _isReleased = true;
        }

        private Vector3 GetPositionFor(EBattleSide side, EWarriorClass @class, int unitIndex)
        {
            if (side == EBattleSide.Enemy)
                @class = 4 - @class;
            var cells = _unitsCells[@class];
            return cells[unitIndex % cells.Length] + transform.position + _offset;
        }

        private int GetIndex(EWarriorClass @class)
        {
            if (_snapped.ContainsKey(@class))
                return _snapped[@class].Count;
            return 0;
        }

        private void AddWarrior(Warrior warrior)
        {
            if (_snapped.ContainsKey(warrior.Class))
                _snapped[warrior.Class].Add(warrior);
            else
                _snapped[warrior.Class] = new List<Warrior>() { warrior };
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _owner == EBattleSide.Ally ? Color.yellow : Color.blue;
            foreach (var cell in _unitsCells[EWarriorClass.Tank])
                Gizmos.DrawSphere(cell + transform.position + _offset, 0.3f);
            Gizmos.color = Color.red;
            foreach (var cell in _unitsCells[EWarriorClass.Soldier])
                Gizmos.DrawSphere(cell + transform.position + _offset, 0.3f);
            Gizmos.color = _owner == EBattleSide.Ally ? Color.blue : Color.yellow;
            foreach (var cell in _unitsCells[EWarriorClass.Mage])
                Gizmos.DrawSphere(cell + transform.position + _offset, 0.3f);
        }
    }
}