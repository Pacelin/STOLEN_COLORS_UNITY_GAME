using System;
using System.Collections.Generic;
using DG.Tweening;
using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPanel : MonoBehaviour
    {
        public GridLayout Grid => _grid;
        
        [SerializeField] private Camera _camera;
        [SerializeField] private GridLayout _grid;
        [SerializeField] private SpriteButton _clearButton;
        [SerializeField] private SpriteButton _applyButton;
        [Space] 
        [SerializeField] private GridPointsConnection _connectionPrefab;
        [SerializeField] private int _maxConnections;
        [Space]
        [SerializeField] private float _errorDuration;
        [SerializeField] private int _errorLoops;
        [SerializeField] private Color _defaultLineColor;
        [SerializeField] private Color _errorLineColor;
        [Space] [SerializeField] private TMP_Text _text;
        [Space] [SerializeField] private float _delay;
        [SerializeField] private float _firstX;
        [SerializeField] private float _secondX;
        [SerializeField] private float _duration;
        
        private List<GridPointsConnection> _connections = new();
        private GridPoint _lastClickedGridPoint;
        private CompositeDisposable _disposables;
        private GridPointsConnection _mouseConnection;
        private Tween _errorTween;

        private AlliesSpawner _spawner;
        private WaveManager _wave;
        private IDisposable _descriptionDisposable;
        private Tween _tween;
        
        [Inject]
        private void Construct(AlliesSpawner spawner, WaveManager wave)
        {
            _grid.Regenerate();
            _wave = wave;
            var transform1 = transform;
            var p = transform1.localPosition;
            p.x = _firstX;
            transform1.localPosition = p;
            _tween = DOTween.Sequence()
                .AppendInterval(_delay)
                .Append(transform.DOLocalMoveX(_secondX, _duration));
            _spawner = spawner;
            _text.text = "";
        }
        
        private void OnEnable()
        {
            _disposables = new();
            _clearButton.OnClick.Subscribe(_ => ClearGrid())
                .AddTo(_disposables);
            _applyButton.OnClick.Subscribe(_ => ApplyGrid())
                .AddTo(_disposables);
            _grid.OnClickPoint.Subscribe(OnClickPoint)
                .AddTo(_disposables);
            GridPoint.OnEnter.Subscribe(OnEnterPoint)
                .AddTo(_disposables);
            GridPoint.OnExit.Subscribe(OnExitPoint)
                .AddTo(_disposables);
            _wave.WaveIsInProgress.Skip(1).Subscribe(w =>
            {
                if (w)
                {
                    ClearGrid();
                    _tween = transform.DOLocalMoveX(_firstX, _duration)
                        .OnComplete(ClearGrid);
                }
                else
                {
                    _tween = transform.DOLocalMoveX(_secondX, _duration);
                }
            }).AddTo(_disposables);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _disposables.Dispose();
            _errorTween?.Kill();
            _descriptionDisposable?.Dispose();
        }

        private void ApplyGrid()
        {
            if (_connections.Count > 0)
            {
                _spawner.Spawn();
                ClearGrid();
                _grid.Regenerate();
            }
        }

        private void ClearGrid()
        {
            _descriptionDisposable?.Dispose();
            _text.text = "";
            _errorTween?.Kill();
            if (_connections.Count > 0)
                foreach (var connection in _connections)
                    Destroy(connection.gameObject);
            if (_mouseConnection)
                Destroy(_mouseConnection.gameObject);
            foreach (var point in _grid)
                point.ResetActivation();

            _connections.Clear();
            _lastClickedGridPoint = null;
            _mouseConnection = null;
        }

        private void OnEnterPoint(GridPoint point)
        {
            _descriptionDisposable = point.Model.Action.GetDescription()
                .Subscribe(s => _text.text = s);
        }

        private void OnExitPoint(GridPoint point)
        {
            _descriptionDisposable?.Dispose();
            _text.text = "";
        }
        
        
        private void OnClickPoint(GridPoint point)
        {
            if (_connections.Count >= _maxConnections)
                return;
            _errorTween?.Kill(true);
            if (_lastClickedGridPoint == null && point.Model.CanConnectThrough)
            {
                _lastClickedGridPoint = point;
                _mouseConnection = Instantiate(_connectionPrefab, transform);
                _mouseConnection.Line.maskInteraction = SpriteMaskInteraction.None;
                Update();
                point.AddActivation();
            }
            else
            {
                var connections = _lastClickedGridPoint.GetConnectionsWith(point);
                if (connections.CanConnect())
                {
                    connections.AddActivations();
                    var newConnection = Instantiate(_connectionPrefab, transform);
                    _connections.Add(newConnection);
                    newConnection.Connect(_lastClickedGridPoint, point);
                    _lastClickedGridPoint = point;
                    
                    if (_connections.Count >= _maxConnections)
                        Destroy(_mouseConnection.gameObject);
                }
                else
                {
                    TweenError();
                }
            }
        }

        private void Update()
        {
            if (_mouseConnection)
            {
                var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _mouseConnection.Connect(_lastClickedGridPoint, mousePos);
            }
        }

        private void TweenError()
        {
            _errorTween?.Kill(true);
            _errorTween = _mouseConnection.Line.DOColor(
                    new Color2(_defaultLineColor, _defaultLineColor),
                    new Color2(_errorLineColor, _errorLineColor), _errorDuration)
                .SetLoops(_errorLoops, LoopType.Yoyo);
        }
    }
}