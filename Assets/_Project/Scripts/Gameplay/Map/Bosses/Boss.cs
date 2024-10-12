using System;
using System.Linq;
using Gameplay.Map.Spawn;
using UnityEngine;
using UniRx;
using Zenject;
using Random = UnityEngine.Random;

namespace Gameplay.Map.Bosses
{
    public class Boss : Unit
    {
        public IObservable<UniRx.Unit> OnActivate => _onActivate;
        private ReactiveCommand _onActivate = new();
        public bool IsActivated => _activated;

        [SerializeField] private BossAnimation _bossAnimation;
        [SerializeField] private BossAttackPoint[] _attackPoints;

        private int _currentAttack;
        private float _counter;
        private bool _activated;
        private CompositeDisposable _disposables;
        private bool _isAttacking;
        
        [Inject] private WarriorsCollection _warriors;
        
        private void OnEnable()
        {
            _isAttacking = false;
            _currentAttack = Random.Range(0, _attackPoints.Length);
            _activated = false;
            
            _disposables = new();
            _bossAnimation.OnAttack
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(index =>
                {
                    _attackPoints[_currentAttack].SendDamage(_warriors.Allies.ToArray());
                }).AddTo(_disposables);
            _bossAnimation.OnAttackFinished
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(index =>
                {
                    _currentAttack = Random.Range(0, _attackPoints.Length);
                    _counter = _attackPoints[_currentAttack].Cooldown;
                    _isAttacking = false;
                }).AddTo(_disposables);
            
            Observable.EveryUpdate()
                .Where(_ => !_isAttacking && _activated && Model.Alive.Value)
                .Subscribe(_ =>
                {
                    _counter -= Time.deltaTime;
                    if (_counter < 0)
                    {
                        _bossAnimation.SetAttack(_currentAttack);
                        _isAttacking = true;
                    }
                })
                .AddTo(_disposables);

            Model.OnDie.Subscribe(_ => _bossAnimation.SetDie())
                .AddTo(_disposables);

            _bossAnimation.OnDie.Subscribe(_ => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            _disposables.Dispose();
        }

        private void Activate()
        {
            _onActivate.Execute();
            _activated = true;
        }
        
        public override void TakeDamage(float damage)
        {
            if (!Model.Alive.Value)
                return;
            Activate();
            base.TakeDamage(damage);
        }
    }
}