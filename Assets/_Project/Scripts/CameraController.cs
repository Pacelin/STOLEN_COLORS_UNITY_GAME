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
    public enum State
    {
        None,
        Auto,
        ShowGoal,
        Manual
    }
    
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _goal;

    [Header("Settings")]
    [SerializeField] private State _initialState = State.Auto;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 10, -8.2f);

    [Header("Animation")] 
    [SerializeField] private Ease _moveTowardsEase = Ease.InFlash;
    [SerializeField] private float _goalMoveDuration = 1.5f;
    [SerializeField] private float _goalStayDuration = 1.5f;
    [SerializeField] private float _interpolationMultiplier = 2;

    [Inject] private WarriorsCollection _warriors;
    [Inject] private CastlesCollection _castles;
    [Inject] private WaveManager _wave;

    private State _state = State.None;
    private IDisposable _stateDisposable;
    private Tween _tween;

    private void OnEnable()
    {
        _cameraTransform.position = GetCameraPositionAt(_start);
        if (_initialState == State.Auto)
            SetAutoState();
        else if (_initialState == State.ShowGoal)
            SetGoalState();
    }

    private void OnDisable()
    {
        _stateDisposable?.Dispose();
        _tween?.Kill();
    }

    public void SetTarget(Transform target) => SetManualState(target);
    
    public void SetAutoState()
    {
        if (_state == State.Auto)
            return;
        _state = State.Auto;
        _tween?.Kill();
        _stateDisposable?.Dispose();

        _stateDisposable = Observable.EveryLateUpdate()
            .Subscribe(_ =>
            {
                if (_wave.WaveIsInProgress.Value)
                {
                    var warrior = _warriors.Allies
                            .OrderByDescending(w => w.Position.x)
                            .FirstOrDefault();
                    if (warrior == null)
                        warrior = _warriors.Enemies
                            .OrderBy(w => w.Position.x)
                            .FirstOrDefault();
                    if (warrior != null)
                        MoveInterpolate(warrior.transform);
                }
                else
                {
                    var castle = _castles.GetCurrentCastle(EBattleSide.Ally);
                    if (castle == null)
                        castle = _castles.GetCurrentCastle(EBattleSide.Enemy);
                    if (castle != null)
                        MoveInterpolate(castle.transform);
                }
            });
    }

    private void SetGoalState()
    {
        if (_state == State.ShowGoal)
            return;
        _state = State.ShowGoal;
        _tween?.Kill();
        _stateDisposable?.Dispose();

        _tween = DOTween.Sequence()
            .Append(MoveTowards(_goal, _goalMoveDuration))
            .AppendInterval(_goalStayDuration)
            .Append(MoveTowards(_castles.GetCurrentCastle(EBattleSide.Ally).transform, _goalMoveDuration))
            .AppendCallback(() => _wave.SetHidePanels(false))
            .AppendCallback(SetAutoState);
    }

    private void SetManualState(Transform target)
    {
        _state = State.Manual;
        _tween?.Kill();
        _stateDisposable?.Dispose();
        
        _stateDisposable = Observable.EveryLateUpdate()
            .Subscribe(_ => MoveInterpolate(target));
    }

    private Tween MoveTowards(Transform target, float duration) =>
        _cameraTransform.DOMoveX(target.position.x, duration)
            .SetEase(_moveTowardsEase);
    private void MoveInterpolate(Transform target) =>
        _cameraTransform.position =
            Vector3.Lerp(_cameraTransform.position, GetCameraPositionAt(target), 
                Time.deltaTime * _interpolationMultiplier);

    private Vector3 GetCameraPositionAt(Transform target) =>
        new Vector3(target.position.x, _cameraOffset.y, _cameraOffset.z);
}