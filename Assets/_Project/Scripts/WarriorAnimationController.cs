using System;
using UniRx;
using UnityEngine;

public class WarriorAnimationController : MonoBehaviour
{
    public IObservable<Unit> OnEmitAttack => _onEmitAttack;
    public IObservable<Unit> OnEmitDie => _onEmitDie;

    private static readonly int s_damageId = Animator.StringToHash("DAMAGE");
    private static readonly int s_attackId = Animator.StringToHash("ATTACK");
    private static readonly int s_deathId = Animator.StringToHash("DEATH");
    private static readonly int s_moveId = Animator.StringToHash("MOVE");
    private static readonly int s_attackSpdId = Animator.StringToHash("ATTACK_SPEED");
    private static readonly int s_moveSpdId = Animator.StringToHash("MOVE_SPEED");
    
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _attackSpeedFactor = 1f, _moveSpeedFactor = 1f;

    private ReactiveCommand<Unit> _onEmitAttack = new();
    private ReactiveCommand<Unit> _onEmitDie = new();

    public void SetIdle()
    {
        if (_animator)
            _animator.SetBool(s_moveId, false);
    }

    public void SetWalk()
    {
        if (_animator)
            _animator.SetBool(s_moveId, true);
    }

    public void Attack()
    {
        if (_animator)
            _animator.SetTrigger(s_attackId);
    }

    public void SetTakeDamage()
    {
        if (_animator)
            _animator.SetTrigger(s_damageId);
    }

    public void Die()
    {
        if (_animator)
            _animator.SetTrigger(s_deathId);
    }

    public void SetAttackSpeed(float value)
    {
        if (_animator)
            _animator.SetFloat(s_attackSpdId, value * _attackSpeedFactor);
    }

    public void SetWalkSpeed(float value)
    {
        if (_animator)
            _animator.SetFloat(s_moveSpdId, value * _moveSpeedFactor);
    }

    public void EmitDamage()
    {
        // Play damage sound
    }
    
    public void EmitAttack()
    {
        _onEmitAttack.Execute(Unit.Default);
        // Play attack sound
    }
    
    public void EmitDie()
    {
        _onEmitDie.Execute(Unit.Default);
        // Play die sound
    }
}
