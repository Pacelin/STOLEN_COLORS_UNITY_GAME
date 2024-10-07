using System;
using UnityEngine;
using UniRx;

namespace Gameplay.Map.Bosses
{
    public class Boss : Unit
    {
        public IObservable<UniRx.Unit> OnActivate => _onActivate;
        private ReactiveCommand _onActivate = new();
        
        private void Start()
        {
            Model.OnDie.Subscribe(_ => GameObject.Destroy(this.gameObject))
                .AddTo(this);
        }

        private void Activate()
        {
            _onActivate.Execute();
        }
        
        public override void TakeDamage(float damage)
        {
            Activate();
            base.TakeDamage(damage);
        }
    }
}