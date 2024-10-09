using System;
using Gameplay.Map.Allies;
using UniRx;
using UnityEngine;

namespace Gameplay.Map
{
    public class UnitModel
    {
        public float Damage => _damage;
        public float MaxHealth => _maxHealth;
        public float AttackSpeed => _attackSpeed;
        public float AttackDistance => _attackDistance;
        public float Speed => _speed;
        
        public IReadOnlyReactiveProperty<float> Health => _health;
        public IReadOnlyReactiveProperty<float> NormalizedHealth => _normalizedHealth;
        public IReadOnlyReactiveProperty<bool> Alive => _alive;
        public IObservable<float> OnTakeDamage => _onTakeDamage;
        public IObservable<UniRx.Unit> OnDie => _alive.Where(a => !a).AsUnitObservable();

        private float _damage;
        private float _maxHealth;
        private float _attackSpeed;
        private float _attackDistance;
        private float _speed;
        private readonly ReactiveProperty<float> _health;
        private readonly IReadOnlyReactiveProperty<float> _normalizedHealth;
        private readonly ReactiveCommand<float> _onTakeDamage;
        private readonly ReactiveProperty<bool> _alive;
        private readonly UnitData _data;
        private readonly EWarriorClass _class;

        public UnitModel(UnitData data, SpawnModifiers modifiers, EWarriorClass @class)
        {
            _data = data;
            _class = @class;
            _speed = data.Speed + modifiers.WalkSpeed;
            _damage = data.Damage * modifiers.DamageMultiplier;
            _maxHealth = data.Health * modifiers.HealthMultiplier;
            _attackSpeed = data.AttackSpeed * modifiers.AttackSpeedMultiplier;
            if (_class == EWarriorClass.Mage)
                _attackDistance = data.AttackDistance + modifiers.MagesAttackRange;
            else
                _attackDistance = data.AttackDistance;
            _health = new(_maxHealth);
            _normalizedHealth = _health.Select(h => h / _maxHealth).ToReactiveProperty();
            _onTakeDamage = new();
            _alive = new(true);
        }

        public void UpdateStats(SpawnModifiers modifiers)
        {
            var newSpeed = _data.Speed + modifiers.WalkSpeed;
            var newDamage = _data.Damage * modifiers.DamageMultiplier;
            var newMaxHealth = _data.Health * modifiers.HealthMultiplier;
            var newAtkSpd = _data.AttackSpeed * modifiers.AttackSpeedMultiplier;
            var newAtkDistance = _data.AttackDistance;
            if (_class == EWarriorClass.Mage)
                newAtkDistance += modifiers.MagesAttackRange;

            var healthDelta = newMaxHealth - _maxHealth;
            _speed = newSpeed;
            _damage = newDamage;
            _maxHealth = newMaxHealth;
            _attackSpeed = newAtkSpd;
            _attackDistance = newAtkDistance;
            _health.Value += healthDelta;
        }
        
        public void TakeDamage(float damage)
        {
            if (_alive.Value)
            {
                _health.Value = Mathf.Max(0, _health.Value - damage);
                if (_health.Value == 0)
                    _alive.Value = false;
                _onTakeDamage.Execute(damage);
            }
        }

        public void Heal(float heal)
        {
            if (_alive.Value)
                _health.Value = Mathf.Min(_maxHealth, _health.Value + heal);
        }
    }
}