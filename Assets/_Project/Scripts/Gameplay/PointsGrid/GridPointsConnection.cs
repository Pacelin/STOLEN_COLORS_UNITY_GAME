using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPointsConnection : MonoBehaviour
    {
        public LineRenderer Line => _line;
        
        [SerializeField] private LineRenderer _line;

        public void Connect(GridPoint first, Vector3 position) =>
            _line.SetPositions(new Vector3[] {first.transform.position, position});
        public void Connect(GridPoint first, GridPoint last) =>
            _line.SetPositions(new Vector3[] {first.transform.position, last.transform.position});
    }
}