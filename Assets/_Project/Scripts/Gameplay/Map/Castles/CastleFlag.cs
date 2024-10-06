using UnityEngine;

namespace Gameplay.Map
{
    public class CastleFlag : MonoBehaviour
    {
        [SerializeField] private Castle _castle;
        [SerializeField] private GameObject _allyFlag;
        [SerializeField] private GameObject _enemyFlag;

        private void Update()
        {
            _allyFlag.SetActive(_castle.Owner == EBattleSide.Ally);
            _enemyFlag.SetActive(_castle.Owner == EBattleSide.Enemy);
        }
    }
}