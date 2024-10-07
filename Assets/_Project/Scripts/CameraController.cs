using DG.Tweening;
using Gameplay.Map;
using Gameplay.Map.Spawn;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject]
    private WarriorsCollection _warriors;
    [Inject]
    private CastlesCollection _castles;
    [Inject]
    private WaveManager _waveManager;

    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private Transform _start, _goal;
    
    [SerializeField]
    private float _initialMoveSpeed = 10f, _cameraGoalStopDelay = 3f, _lerpSpeed = 1f;

    private Tween _tween;

    [SerializeField]
    private Vector3 _cameraOffset = new(0f, 10f, 0f), _castleOffset = new(5f, 0f, 0f);

    private bool _update = false;

    private Func<Vector3> _getCameraTargetPosition;

    private CompositeDisposable _disposables = new();
    
    private void OnEnable()
    {
        ShowLevelGoal();

        _waveManager.WaveIsInProgress.Subscribe(value => _getCameraTargetPosition = value ? GetMostRightAllyPosition : GetAllyCastlePosition).AddTo(_disposables);
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }

    public void ShowLevelGoal()
    {
        float distance = Mathf.Abs(_start.position.x - _goal.position.x);
        float time = distance / _initialMoveSpeed;

        _tween?.Kill();
        
        _tween = DOTween.Sequence().
                Append(_camera.DOMove(_goal.position + _cameraOffset, time)).
                AppendInterval(_cameraGoalStopDelay).
                Append(_camera.DOMove(_start.position + _cameraOffset, time)).
                SetEase(Ease.InOutFlash).
                AppendCallback(() => _update = true);
    }

    private void LateUpdate()
    {
        if (!_update)
            return;
        
        MoveCamera();
    }

    private void MoveCamera()
    {
        var allyPosition = _getCameraTargetPosition();
        var targetPosition = allyPosition + _cameraOffset;
        targetPosition.x = Mathf.Clamp(targetPosition.x, _start.position.x, _goal.position.x);

        var cameraPosition = _camera.position;
        cameraPosition.x = Mathf.Lerp(_camera.position.x, targetPosition.x, Time.deltaTime * _lerpSpeed);

        _camera.position = cameraPosition;
    }

    private Vector3 GetAllyCastlePosition()
    {
        var castle = _castles.GetCastle(EBattleSide.Ally);
        return (castle ? castle.transform.position : _start.position) + _castleOffset;
    }
    
    private Vector3 GetMostRightAllyPosition()
    {
        var warrior = _warriors.Allies.OrderByDescending(warrior => warrior.transform.position.x).FirstOrDefault();
        return warrior ? warrior.transform.position : _start.position;
    }
}
