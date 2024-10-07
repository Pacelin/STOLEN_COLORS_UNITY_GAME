using DG.Tweening;
using Gameplay.Map.Spawn;
using System.Linq;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject]
    private WarriorsCollection _warriors;

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

    private void OnEnable()
    {
        ShowLevelGoal();
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
        // когда перерыв, берём позицию замка + оффсет замка + оффсет камеры
        // когда игра, берём позицию союзника + оффсет камеры
        var allyPosition = GetMostRightAllyPosition();
        var targetPosition = allyPosition + _cameraOffset;

        _camera.position = Vector3.Lerp(_camera.position, targetPosition, Time.deltaTime * _lerpSpeed);
    }

    private Vector3 GetMostRightAllyPosition()
    {
        var warrior = _warriors.Allies.OrderByDescending(warrior => warrior.transform.position.x).FirstOrDefault();
        return warrior ? warrior.transform.position : _start.position;
    }
}
