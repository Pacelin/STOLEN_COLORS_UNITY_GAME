using Audio.Gameplay.PointsGrid;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TutorialArmController : MonoBehaviour
{
    [SerializeField]
    private GridPanel _gridPanel;
    
    [SerializeField]
    private Vector2Int[] _points;

    [SerializeField]
    private float _initialDelay = 5f;
    
    [SerializeField]
    private float _armMoveTime = .3f, _armStayTime = 1f, _restartDelay = 2f;

    [SerializeField]
    private float _tapScale = .8f, _tapTime = .1f;

    private Sequence _sequence;

    private CompositeDisposable _disposables;

    [SerializeField]
    private Vector3 _positionOffset;
    
    private async UniTaskVoid Start()
    {
        _gridPanel.Grid.Regenerate(ignoreBadPoints: true);
        
        transform.localPosition = _gridPanel.Grid.Get(_points[0].x, _points[0].y).transform.localPosition + _positionOffset;

        await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

        _sequence = DOTween.Sequence();

        for (int i = 1; i < _points.Length; i++)
        {
            _sequence.
                AppendInterval(_armStayTime).
                Append(transform.DOScale(Vector3.one * _tapScale, _tapTime)).
                Append(transform.DOScale(Vector3.one, _tapTime)).
                AppendInterval(_armStayTime).
                Append(transform.DOLocalMove(_gridPanel.Grid.Get(_points[i].x, _points[i].y).transform.localPosition + _positionOffset, _armMoveTime));
        }

        _sequence.
            AppendInterval(_armStayTime).
            Append(transform.DOScale(Vector3.one * _tapScale, _tapTime)).
            Append(transform.DOScale(Vector3.one, _tapTime)).
            AppendInterval(_restartDelay).
            AppendCallback(() => transform.localPosition = _gridPanel.Grid.Get(_points[0].x, _points[0].y).transform.localPosition + _positionOffset).
            SetLoops(-1);

        _disposables = new CompositeDisposable();
        _gridPanel.OnApply.Subscribe(OnApply).AddTo(_disposables);
    }
    
    private void OnDestroy()
    {
        _sequence?.Kill();
        _disposables?.Dispose();
    }
    
    private void OnApply(IEnumerable<GridPoint> _)
    {
        Destroy(gameObject);
    }
}
