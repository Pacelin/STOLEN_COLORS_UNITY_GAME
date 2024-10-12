using DG.Tweening;
using Gameplay.Map;
using Gameplay.Map.Spawn;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
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
    private float _goalMoveTime = 3f;

    [SerializeField]
    private float _cameraGoalStopDelay = 3f;

    private Tween _tween;

    [SerializeField]
    private Vector3 _cameraOffset = new(0f, 10f, 0f), _castleOffset = new(5f, 0f, 0f), _creatureOffset = new(5f, 0f, 0f);

    [SerializeField]
    private bool _showLevelGoal;
    
    private Transform _currentTarget;
    private Vector3 _currentTargetPosition;
    private Vector3 _targetOffset;
    
    [SerializeField]
    private float _lerpSpeed = 1f;

    private bool _update = false;
    
    private CompositeDisposable _disposables = new();

    private CompositeDisposable _warriorDieDisposable;
    
    private void OnEnable()
    {
        _waveManager.WaveIsInProgress.Subscribe(UpdateCurrentTarget).AddTo(_disposables);
        
        if (_showLevelGoal)
            ShowLevelGoal();
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }

    private void LateUpdate()
    {
        if (_update)
            MoveCamera();
    }

    private void ShowLevelGoal()
    {
        _tween?.Kill();
        _tween = DOTween.Sequence().
                Append(_camera.DOMoveX(_goal.position.x, _goalMoveTime)).
                AppendInterval(_cameraGoalStopDelay).
                Append(_camera.DOMoveX(_start.position.x + _castleOffset.x, _goalMoveTime)).
                AppendCallback(() => SetUpdate(true)).
                SetEase(Ease.InFlash);
    }

    private Vector3 GetTargetPosition()
    {
        if (_currentTarget)
            return _currentTarget.position + _targetOffset;
        return _currentTargetPosition + _targetOffset;
    }

    public void SetUpdate(bool value) => _update = value;
    
    public void SetTarget(Transform target)
    {
        _currentTarget = target;
    }
    
    public void SetTarget(Vector3 targetPosition)
    {
        _currentTarget = null;
        _currentTargetPosition = targetPosition;
    }
    
    public void MoveToTarget(Transform target, float time)
    {
        _tween = DOTween.Sequence();
    }

    public void MoveToTarget(Vector3 targetPosition, float time)
    {
        _tween?.Kill();
        _tween = DOTween.Sequence(_camera.DOMoveX(targetPosition.x, time));
    }

    public void SetTargetOffset(Vector3 targetOffset)
    {
        _targetOffset = targetOffset;
        _targetOffset.y = 0f;
        _targetOffset.z = 0f;
    }
    
    private void MoveCamera()
    {
        var targetPosition = GetTargetPosition();

        var x = Mathf.Lerp(transform.position.x, targetPosition.x, Time.smoothDeltaTime * _lerpSpeed);

        Vector3 currentPosition = _camera.position;
        currentPosition.x = x;
        currentPosition.y = 0f;
        currentPosition.z = 0f;
        
        _camera.position = currentPosition + _cameraOffset;
    }

    private void UpdateCurrentTarget(bool waveStart)
    {
        if (waveStart)
            SetTargetAllyCreature();
        else
            SetTargetCastle();
    }

    private void SetTargetCastle()
    {
        var castle = _castles.GetCurrentCastle(EBattleSide.Ally);

        if (castle)
        {
            SetTarget(castle.transform);
            SetTargetOffset(_castleOffset);
        }
        else
        {
            SetTarget(_start);
            SetTargetOffset(Vector3.zero);
        }
    }
    
    private void SetTargetAllyCreature()
    {
        var warrior = _warriors.Allies.OrderByDescending(w => w.transform.position.x).FirstOrDefault();
        if (warrior)
        {
            SetTarget(warrior.transform);
            SetTargetOffset(_creatureOffset);
            _targetOffset = _creatureOffset;

            _warriorDieDisposable?.Dispose();
            _warriorDieDisposable = new CompositeDisposable();
            warrior.OnDie.Subscribe(_ => SetTargetAllyCreature()).AddTo(_warriorDieDisposable);
            return;
        }

        var enemy = _warriors.Enemies.OrderBy(w => w.transform.position.x).FirstOrDefault();
        if (enemy)
        {
            SetTarget(enemy.transform);
            SetTargetOffset(-_creatureOffset);
            return;
        }
        SetTargetCastle();
    }
}
