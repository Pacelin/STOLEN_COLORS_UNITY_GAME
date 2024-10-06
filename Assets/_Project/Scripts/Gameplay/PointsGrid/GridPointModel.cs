using UniOwl;
using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPointModel : MonoBehaviour
    {
        public bool CanConnectThrough => _canConnectThrough;
        public IGridPointAction Action => _action;
        
        [SerializeField] private bool _canConnectThrough;
        [Dropdown(false, true)]
        [SerializeReference] private IGridPointAction _action;
    }
}