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
    
    private Vector3 _initialPosition;
    private Transform _currentTarget;
    private Vector3 _currentTargetPosition;
    private Vector3 _targetOffset;
    private float _moveTime;
    
    private CompositeDisposable _disposables = new();
    
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
        MoveCamera();
    }

    private void ShowLevelGoal()
    {
        _tween?.Kill();
        _tween = DOTween.Sequence().
                AppendCallback(() => SetTarget(_goal, _goalMoveTime)).
                AppendInterval(_goalMoveTime + _cameraGoalStopDelay).
                AppendCallback(() => SetTarget(_start, _goalMoveTime));
    }

    private Vector3 GetTargetPosition()
    {
        if (_currentTarget)
            return _currentTarget.position + _targetOffset;
        return _currentTargetPosition + _targetOffset;
    }

    public void SetTarget(Transform target)
    {
        _initialPosition = transform.position;
        _currentTarget = target;
    }
    
    public void SetTarget(Vector3 targetPosition)
    {
        _initialPosition = transform.position;
        _currentTarget = null;
        _currentTargetPosition = targetPosition;
    }
    
    public void SetTarget(Transform target, float time)
    {
        _initialPosition = transform.position;
        
        _currentTarget = target;
        
        _moveTime = time;
    }

    public void SetTarget(Vector3 targetPosition, float time)
    {
        _initialPosition = transform.position;
        
        _currentTarget = null;
        _currentTargetPosition = targetPosition;
        
        _moveTime = time;
    }

    public void SetTargetOffset(Vector3 targetOffset)
    {
        _targetOffset = targetOffset;
    }
    
    private void MoveCamera()
    {
        Debug.Log(_currentTarget);
        
        var targetPosition = GetTargetPosition();
        var currentPosition = Vector3.Lerp(_initialPosition, targetPosition, Time.deltaTime / _moveTime);

        Debug.Log(Vector3.Lerp(_initialPosition, targetPosition, Time.deltaTime / _moveTime));
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
