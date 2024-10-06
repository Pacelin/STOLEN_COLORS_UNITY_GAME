using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPointModel : MonoBehaviour
    {
        public bool CanConnectThrough => _canConnectThrough;

        [SerializeField] private bool _canConnectThrough;
    }
}