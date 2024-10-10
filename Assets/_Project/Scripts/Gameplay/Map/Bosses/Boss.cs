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
        [SerializeField] private float[] _attacksCooldown;
        [SerializeField] private float[] _attacksRadiuses;
        [SerializeField] private float[] _attacksDamages;

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
            _currentAttack = Random.Range(0, _attacksCooldown.Length);
            _activated = false;
        }

        private void OnEnable()
        {
            _disposables = new();
            _bossAnimation.OnAttack
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(index =>
                {
                    var attackedWarriors = GetAttackedWarriors(index);
                    foreach (var attackedWarrior in attackedWarriors)
                        attackedWarrior.TakeDamage(_attacksDamages[index]);
                    _currentAttack = _nextAttack;
                }).AddTo(_disposables);
            
            Observable.EveryUpdate()
                .Where(_ => _activated && Model.Alive.Value)
                .Subscribe(_ =>
                {
                    _counter -= Time.deltaTime;
                    if (_counter < 0 && GetAttackedWarriors(_currentAttack).Length >= 1)
                    {
                        _bossAnimation.SetAttack(_currentAttack);
                        _nextAttack = Random.Range(0, _attacksCooldown.Length);
                        _counter = _attacksCooldown[_nextAttack];
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

        private Warrior[] GetAttackedWarriors(int index)
        {
            if (_warriors.Allies.Count == 0)
                return Array.Empty<Warrior>();
            var ordered = _warriors.Allies.OrderBy(w =>
                Vector3.Distance(transform.position, w.Position)).ToArray();
            if (Vector3.Distance(transform.position, ordered[0].Position) > _attacksRadiuses[index])
                return new Warrior[] { ordered[0] };
            return ordered.Where(w =>
                    Vector3.Distance(transform.position, w.Position) <= _attacksRadiuses[index])
                .ToArray();
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            foreach (var radius in _attacksRadiuses)
                Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}