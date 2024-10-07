using System.Linq;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class CastlesCollection : MonoBehaviour
    {
        public Castle CapturingCastle => _capturingCastle;
        public bool HasAllyCastle => _castles.Any(c => c.Owner == EBattleSide.Ally);
        
        [SerializeField] private Castle[] _castles;

        private Castle _allyCastle;
        private Castle _enemyCastle;
        private Castle _capturingCastle;

        [Inject]
        private void Construct()
        {
            _allyCastle = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastle = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
        }

        public void SetCapturingCastle(Castle castle) => _capturingCastle = castle;
        
        public Castle GetCastle(EBattleSide side, bool includeCapturing = false)
        {
            if (includeCapturing && _capturingCastle)
            {
                if (_capturingCastle.Owner == side)
                    return _capturingCastle;
                if (side == EBattleSide.Ally)
                    return _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
                return _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
            }
            if (includeCapturing && _capturingCastle && _capturingCastle.Owner == side)
                return _capturingCastle;
            if (side == EBattleSide.Ally)
                return _allyCastle;
            return _enemyCastle;
        }
        
        public void SnapCastles()
        {
            foreach (var castle in _castles)
                castle.SnapUnits();
        }

        public void ReleaseCastles()
        {
            _capturingCastle = null;
            _allyCastle = _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            _enemyCastle = _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
            if (_allyCastle)
                _allyCastle.ReleaseUnits();
            if (_enemyCastle)
                _enemyCastle.ReleaseUnits();
        }
    }
}