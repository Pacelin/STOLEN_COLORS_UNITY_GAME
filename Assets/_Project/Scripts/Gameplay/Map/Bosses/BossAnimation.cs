using System;
using UniRx;
using UnityEngine;

namespace Gameplay.Map.Bosses
{
    public class BossAnimation : MonoBehaviour
    {
        public IObservable<int> OnAttack => _onAttack;
        public IObservable<UniRx.Unit> OnDie => _onDie;

        private static readonly int ATTACK_INDEX = Animator.StringToHash("attack_index");
        private static readonly int ATTACK = Animator.StringToHash("attack");
        private static readonly int DIE = Animator.StringToHash("die");
        
        [SerializeField] private Animator _animator;
        
        private ReactiveCommand<int> _onAttack = new();
        private ReactiveCommand _onDie = new();
        
        public void AttackEvent(int attackIndex) =>
            _onAttack.Execute(attackIndex);
        public void DieEvent() =>
            _onDie.Execute();

        public void SetAttack(int index)
        {
            _animator.SetInteger(ATTACK_INDEX, index);
            _animator.SetTrigger(ATTACK);
        }

        public void SetDie()
        {
            _animator.SetTrigger(DIE);
        }
    }
}