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
        private int _nextAttack;
        private float _counter;
        private bool _activated;
        private CompositeDisposable _disposables;
        
        [Inject] private WarriorsCollection _warriors;
        
        private void Start()
        {
            Model.OnDie.Subscribe(_ => GameObject.Destroy(this.gameObject))
                .AddTo(this);
            _currentAttack = Random.Range(0, _attackPoints.Length);
            _activated = false;
        }

        private void OnEnable()
        {
            _disposables = new();
            _bossAnimation.OnAttack
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(index =>
                {
                    _attackPoints[_currentAttack].SendDamage(_warriors.Allies.ToArray());
                    _currentAttack = _nextAttack;
                }).AddTo(_disposables);
            
            Observable.EveryUpdate()
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(_ =>
                {
                    _counter -= Time.deltaTime;
                    if (_counter < 0)
                    {
                        _bossAnimation.SetAttack(_currentAttack);
                        _nextAttack = Random.Range(0, _attackPoints.Length);
                        _counter = _attackPoints[_nextAttack].Cooldown;
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