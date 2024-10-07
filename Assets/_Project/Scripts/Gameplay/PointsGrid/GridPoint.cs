using System;
using AYellowpaper.SerializedCollections;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPoint : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public static IObservable<GridPoint> OnClick => _onClick;
        public static IObservable<GridPoint> OnEnter => _onEnter;
        public static IObservable<GridPoint> OnExit => _onExit;
        
        private static readonly ReactiveCommand<GridPoint> _onClick = new();
        private static readonly ReactiveCommand<GridPoint> _onEnter = new();
        private static readonly ReactiveCommand<GridPoint> _onExit = new();
        
        public Vector2Int Coordinates
        {
            get => _coordinates;
            set => _coordinates = value;
        }
        public GridLayout Layout
        {
            get => _layout;
            set => _layout = value;
        }

        public GridPointModel Model => _model;
        public int ActivationsCount => _activationsCount;
        public float Weight => _weight;
        
        [SerializeField] private GridPointModel _model;
        [SerializeField] private SerializedDictionary<int, GameObject> _activeMarkers;
        [SerializeField] private float _weight;
        
        private Vector2Int _coordinates;
        private GridLayout _layout;
        private int _activationsCount = 0;

        private void Awake()
        {
            foreach (var marker in _activeMarkers)
                marker.Value.SetActive(false);
        }

        public void AddActivation()
        {
            if (_activationsCount >= 3)
                return;
            if (_activeMarkers.ContainsKey(_activationsCount))
                _activeMarkers[_activationsCount].SetActive(false);
            _activationsCount++;
            if (_activeMarkers.ContainsKey(_activationsCount))
                _activeMarkers[_activationsCount].SetActive(true);
        }

        public void ResetActivation()
        {
            if (_activeMarkers.ContainsKey(_activationsCount))
                _activeMarkers[_activationsCount].SetActive(false);
            _activationsCount = 0;  
        }

        public void OnPointerClick(PointerEventData eventData) =>
            _onClick.Execute(this);
        public void OnPointerEnter(PointerEventData eventData) =>
            _onEnter.Execute(this);
        public void OnPointerExit(PointerEventData eventData) =>
            _onExit.Execute(this);
    }
}