using System;
using Audio;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class Warrior : Unit
    {
        public IObservable<UniRx.Unit> OnDie => Model.OnDie;
        public Vector3 SnapPosition => _snapPosition;
        public Unit AttackTarget => _attackTarget;
        public WarriorAnimationController Animation => _animation;
        
        private WarriorStateMachine _stateMachine;
        private Unit _attackTarget;
        private Vector3 _snapPosition;
        private IDisposable _attackDisposable;
        private CompositeDisposable _dieDisposables;
        
        [SerializeField] private WarriorAnimationController _animation;
        [SerializeField] private Transform _holder;

        [Inject] private AudioSystem _audio;

        [Inject]
        private void Construct(DiContainer container)
        {
            _stateMachine = new WarriorStateMachine(container, this);
        }

        public void Release() => _stateMachine.SwitchState<WarriorWalkToCastleState>();

        public void Snap(Vector3 position)
        {
            _snapPosition = position;
            _stateMachine.SwitchState<WarriorWaitSignalState>();
        }

        public void SetAttack(Unit warrior)
        {
            _attackTarget = warrior;
            _stateMachine.SwitchState<WarriorAttackEnemyState>();
        }

        public void ApplyAttack(Unit warrior)
        {
            _attackDisposable?.Dispose();
            _attackDisposable = _animation.OnEmitAttack
                .First()
                .Subscribe(_ =>
                {
                    warrior.TakeDamage(Model.Damage);
                });
            _animation.Attack();
            if (warrior.Side == EBattleSide.Ally)
            {
                if (warrior.@Class == EWarriorClass.Mage)
                    _audio.PlaySound(ESoundKey.AttackMagicAlly);
                else
                    _audio.PlaySound(ESoundKey.AttackMeleeAlly);
            }
            else
            {
                if (warrior.@Class == EWarriorClass.Mage)
                    _audio.PlaySound(ESoundKey.AttackMagicEnemy);
                else
                    _audio.PlaySound(ESoundKey.AttackMeleeEnemy);
            }
        }

        public override void TakeDamage(float damage)
        {
            _animation.SetTakeDamage();
            base.TakeDamage(damage);
        }

        public void SetWinner()
        {
            _animation.SetIdle();
            _stateMachine.SwitchState<WarriorWinState>();
        }

        private void Update()
        {
            _stateMachine.Update();
            _holder.localPosition = new Vector3(0, 4 - (transform.position.z + 14) , 0);
        }
        private void FixedUpdate() => _stateMachine.FixedUpdate();

        private void OnEnable()
        {
            _stateMachine.Run();
            _dieDisposables = new();
            _animation.OnEmitDie.Subscribe(_ => GameObject.Destroy(gameObject))
                .AddTo(_dieDisposables);
            Model.OnDie.Subscribe(_ => _animation.Die())
                .AddTo(_dieDisposables);
        }

        private void OnDisable()
        {
            _stateMachine.Stop();
            _dieDisposables.Dispose();
            _attackDisposable?.Dispose();
        }
    }
}