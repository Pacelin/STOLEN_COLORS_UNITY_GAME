﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class GridPanel : MonoBehaviour
    {
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
        
        private List<GridPointsConnection> _connections = new();
        private GridPoint _lastClickedGridPoint;
        private CompositeDisposable _disposables;
        private GridPointsConnection _mouseConnection;
        private Tween _errorTween;
        
        [Inject]
        private void Construct() =>
            _grid.Regenerate();
        
        private void OnEnable()
        {
            _disposables = new();
            _clearButton.OnClick.Subscribe(_ => ClearGrid())
                .AddTo(_disposables);
            _applyButton.OnClick.Subscribe(_ => ApplyGrid())
                .AddTo(_disposables);
            _grid.OnClickPoint.Subscribe(OnClickPoint)
                .AddTo(_disposables);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
            _errorTween?.Kill();
        }

        private void ApplyGrid()
        {
        }

        private void ClearGrid()
        {
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

        private void OnClickPoint(GridPoint point)
        {
            _errorTween?.Kill(true);
            if (_lastClickedGridPoint == null)
            {
                _lastClickedGridPoint = point;
                _mouseConnection = Instantiate(_connectionPrefab, transform);
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