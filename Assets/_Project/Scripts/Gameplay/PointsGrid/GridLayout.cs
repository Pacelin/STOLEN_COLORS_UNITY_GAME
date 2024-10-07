using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Map.Enemies;
using UniRx;
using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public class GridLayout : MonoBehaviour, IEnumerable<GridPoint>
    {
        public IObservable<GridPoint> OnClickPoint => _onClickPoint;

        [SerializeField] private Vector2Int _size;
        [SerializeField] private GridPoint[] _gridPointPrefabs;
        [SerializeField] private Vector2Int[] _disabledCoordinates;
        [SerializeField] private float _pointSize;

        private List<GridPoint> _gridPoints;
        private ReactiveCommand<GridPoint> _onClickPoint = new();
        private CompositeDisposable _disposables;
        
        public void Regenerate()
        {
            _disposables?.Dispose();
            _disposables = new();
            
            if (_gridPointPrefabs == null || _gridPointPrefabs.Length == 0)
                return;

            if (_gridPoints == null)
                _gridPoints = new();
            
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            
            _gridPoints.Clear();
            var size = _size.x * _size.y;
            for (int i = 0; i < size; i++)
                _gridPoints.Add(Instantiate(_gridPointPrefabs.GetRandom(), transform));

            var wDistance = _pointSize * Mathf.Sqrt(3);
            var hDistance = _pointSize * 1.5f;
            for (int i = 0; i < _size.x; i++)
            {
                for (int j = 0; j < _size.y; j++)
                {
                    var pos = new Vector3(i * wDistance + j % 2 * wDistance * 0.5f, 0, j * hDistance);
                    var point = _gridPoints[i + j * _size.x];
                    var coords = ConvertCoordinates(i, j);
                    if (_disabledCoordinates.Contains(coords))
                        point.gameObject.SetActive(false);
                    point.transform.localPosition = pos;
                    point.gameObject.name = $"({coords})";
                    point.Coordinates = coords;
                    point.Layout = this;
                }
            }

            GridPoint.OnClick.Subscribe(p => _onClickPoint.Execute(p))
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables?.Dispose();
        }

        public GridPoint Get(int x, int y)
        {
            x = (x - y % 2) / 2;
            return _gridPoints[x + y * _size.x];
        }
        
        private Vector2Int ConvertCoordinates(int x, int y) =>
            new Vector2Int(y % 2 + x * 2, y);

        public IEnumerator<GridPoint> GetEnumerator()
        {
            foreach (var gridPoint in _gridPoints)
                if (gridPoint.gameObject.activeSelf)
                    yield return gridPoint;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}