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
    private int[] _pointIndices;

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
    
    private async UniTaskVoid Awake()
    {
        _gridPanel.Grid.Regenerate(ignoreBadPoints: true);
        
        List<GridPoint> points = new();
        foreach (var point in _gridPanel.Grid)
            points.Add(point);

        transform.localPosition = points[0].transform.localPosition + _positionOffset;

        await UniTask.Delay(TimeSpan.FromSeconds(_initialDelay));

        _sequence = DOTween.Sequence();

        for (int i = 1; i < _pointIndices.Length; i++)
        {
            int point = _pointIndices[i];
            _sequence.
                AppendInterval(_armStayTime).
                Append(transform.DOScale(Vector3.one * _tapScale, _tapTime)).
                Append(transform.DOScale(Vector3.one, _tapTime)).
                AppendInterval(_armStayTime).
                Append(transform.DOLocalMove(points[point].transform.localPosition + _positionOffset, _armMoveTime));
        }

        _sequence.
            AppendInterval(_armStayTime).
            Append(transform.DOScale(Vector3.one * _tapScale, _tapTime)).
            Append(transform.DOScale(Vector3.one, _tapTime)).
            AppendInterval(_restartDelay).
            AppendCallback(() => transform.localPosition = points[0].transform.localPosition + _positionOffset).
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
