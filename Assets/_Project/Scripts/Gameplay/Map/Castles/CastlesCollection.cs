using System.Linq;
using UnityEngine;

namespace Gameplay.Map
{
    public class CastlesCollection : MonoBehaviour
    {
        [SerializeField] private Castle[] _castles;

        public Castle GetCastle(EBattleSide side)
        {
            if (side == EBattleSide.Ally)
                return _castles.LastOrDefault(c => c.Owner == EBattleSide.Ally);
            return _castles.FirstOrDefault(c => c.Owner == EBattleSide.Enemy);
        }
    }
}