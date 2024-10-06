using UnityEngine;

public class WarriorAnimationController : MonoBehaviour
{
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
    
    public void SetIdle()
    {
        _animator.SetBool(s_moveId, false);
    }

    public void SetWalk()
    {
        _animator.SetBool(s_moveId, true);
    }

    public void Attack()
    {
        _animator.SetTrigger(s_attackId);
    }

    public void TakeDamage()
    {
        _animator.SetTrigger(s_damageId);
    }

    public void Die()
    {
        _animator.SetTrigger(s_deathId);
    }

    public void SetAttackSpeed(float value)
    {
        _animator.SetFloat(s_attackSpdId, value * _attackSpeedFactor);
    }

    public void SetWalkSpeed(float value)
    {
        _animator.SetFloat(s_moveSpdId, value * _moveSpeedFactor);
    }

    public void EmitDamage()
    {
        // Play damage sound
    }
    
    public void EmitAttack()
    {
        // Play attack sound
    }
    
    public void EmitDie()
    {
        // Play die sound
    }
}
