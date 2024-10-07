using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPointsConnection : MonoBehaviour
    {
        public LineRenderer Line => _line;
        
        [SerializeField] private LineRenderer _line;

        private GridPoint _first;
        private GridPoint _second;
        
        public void Connect(GridPoint first, Vector3 v2)
        {
            var v1 = first.transform.position;
            v1.y = 0.5f;
            v2.y = 0.5f;
            _line.SetPositions(new Vector3[] {v1, v2});
        }
        public void Connect(GridPoint first, GridPoint last)
        {
            _first = first;
            _second = last;
            Update();
        }

        private void Update()
        {
            if (_first)
            {
                var v1 = _first.transform.position;
                v1.y = 0.5f;
                var v2 = _second.transform.position;
                v2.y = 0.5f;
                _line.SetPositions(new Vector3[] {v1, v2});
            }
        }
    }
}