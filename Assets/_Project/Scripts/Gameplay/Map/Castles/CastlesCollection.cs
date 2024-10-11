using System.Linq;
using UnityEngine;

namespace Gameplay.Map
{
    public class CastlesCollection : MonoBehaviour
    {
        public bool HasAllyCastle => _castles.Any(c => c.Owner == EBattleSide.Ally);
        public bool HasEnemyCastle => _castles.Any(c => c.Owner == EBattleSide.Enemy);
        
        [SerializeField] private Castle[] _castles;

        private Castle _allyCastleWave;
        private Castle _enemyCastleWave;

        public void UpdateWaveCastle()
        {
            _allyCastleWave = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastleWave = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
        }

        public Castle GetCurrentCastle(EBattleSide side)
        {
            if (side == EBattleSide.Ally) 
                return _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            return _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
        }

        public Castle GetWaveCastle(EBattleSide side)
        {
            if (side == EBattleSide.Ally)
                return _allyCastleWave;
            return _enemyCastleWave;
        }

        public void SnapCastles()
        {
            foreach (var castle in _castles)
                castle.SnapUnits();
        }

        public void StartWaveCastles()
        {
            UpdateWaveCastle();
            if (_allyCastleWave)
                _allyCastleWave.ReleaseUnits();
            if (_enemyCastleWave)
                _enemyCastleWave.ReleaseUnits();
        }
    }
}