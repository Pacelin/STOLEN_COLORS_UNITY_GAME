using System;
using Audio;
using Gameplay.Map.Bosses;
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
        [SerializeField] private Transform _mageFirePoint;
        [SerializeField] private MageProjectile _mageProjectilePrefab;
        
        [Inject] private AudioSystem _audio;
        [Inject] private CastlesCollection _castles;
        [Inject] private BossReference _boss;

        [Inject]
        private void Construct(DiContainer container)
        {
            _stateMachine = new WarriorStateMachine(container, this);
        }

        public void Release()
        {
            if (_castles.HasEnemyCastle)
                _stateMachine.SwitchState<WarriorWalkToCastleState>();
            else
                SetAttack(_boss.Boss);
        }

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
            
            if (Class == EWarriorClass.Mage)
                MageAttack(warrior);
            else
                MeleeAttack(warrior);
        }

        private void MageAttack(Unit target)
        {
            _attackDisposable = _animation.OnEmitAttack
                .First()
                .Subscribe(_ =>
                {
                    if (target)
                    {
                        var position = _mageFirePoint.position;
                        var proj = Instantiate(_mageProjectilePrefab, position, Quaternion.identity);
                        proj.Shoot(position, target, Model.Damage);
                        if (Side == EBattleSide.Ally)
                            _audio.PlaySound(ESoundKey.AttackMagicAlly);
                        else
                            _audio.PlaySound(ESoundKey.AttackMagicEnemy);
                    }
                });
            _animation.Attack();
        }

        private void MeleeAttack(Unit target)
        {
            _attackDisposable = _animation.OnEmitAttack
                .First()
                .Subscribe(_ =>
                {
                    if (Side == EBattleSide.Ally)
                        _audio.PlaySound(ESoundKey.AttackMeleeAlly);
                    else
                        _audio.PlaySound(ESoundKey.AttackMeleeEnemy);
                    target.TakeDamage(Model.Damage);
                });
            _animation.Attack();
        }

        public override void TakeDamage(float damage)
        {
            if (!Model.Alive.Value)
                return;
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
            Model.OnDie.Subscribe(_ =>
                {
                    _stateMachine.Stop();
                    _animation.Die();
                })
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