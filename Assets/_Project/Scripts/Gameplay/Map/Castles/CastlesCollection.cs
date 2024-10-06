using System;
using System.Linq;
using Gameplay.Map.Enemies;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class CastlesCollection : MonoBehaviour
    {
        [SerializeField] private Castle[] _castles;

        private Castle _allyCastle;
        private Castle _enemyCastle;

        [Inject]
        private void Construct()
        {
            _allyCastle = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastle = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
        }

        public Castle GetCastle(EBattleSide side)
        {
            if (side == EBattleSide.Ally)
                return _allyCastle;
            return _enemyCastle;
        }
        
        public void SnapCastles()
        {
            _allyCastle = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastle = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
            foreach (var castle in _castles)
                castle.SnapUnits();
        }

        public void ReleaseCastles()
        {
            _allyCastle = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastle = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
            if (_allyCastle)
                _allyCastle.ReleaseUnits();
            if (_enemyCastle)
                _enemyCastle.ReleaseUnits();
        }
    }
}