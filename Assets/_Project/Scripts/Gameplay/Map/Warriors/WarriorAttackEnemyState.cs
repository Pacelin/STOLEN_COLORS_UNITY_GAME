using UnityEngine;

namespace Gameplay.Map
{
    public class WarriorAttackEnemyState : WarriorState
    {
        private const float UPDATE_DESTINATION_THRESHOLD = 0.1f;
        private float _counter;
        private float _lastTime;
        
        public override void Enter()
        {
            _warrior.Agent.stoppingDistance = 
                _warrior.Model.AttackDistance + _warrior.Agent.radius + _warrior.AttackTarget.Agent.radius;
            _counter -= Time.time - _lastTime;
        }

        public override void FixedUpdate()
        {
            _counter -= Time.fixedDeltaTime;
            if (!_warrior.AttackTarget || !_warrior.AttackTarget.Model.Alive.Value)
            {
                _warrior.Release();
                return;
            }
            
            var targetPosition = _warrior.AttackTarget.Position;
            var distanceBetweenWarriors = Vector3.Distance(_warrior.Position, targetPosition);
            if (distanceBetweenWarriors <= _warrior.Model.AttackDistance + _warrior.Agent.radius + _warrior.AttackTarget.Agent.radius + UPDATE_DESTINATION_THRESHOLD)
                AttackFixedUpdate();
            else
            {
                if (_warrior.Agent.isStopped ||
                    Vector3.Distance(targetPosition, _warrior.Agent.destination) > UPDATE_DESTINATION_THRESHOLD)
                {
                    _warrior.Animation.SetWalk();
                    _warrior.Agent.ResetPath();
                    _warrior.Agent.SetDestination(targetPosition);
                }
            }
        }

        public override void Exit()
        {
            _lastTime = Time.time;
        }
        
        private void AttackFixedUpdate()
        {
            if (_counter <= 0)
            {
                var cooldown = 1f / _warrior.Model.AttackSpeed;
                var animTime = 0.6f;
                if (_warrior.Class == EWarriorClass.Mage)
                    animTime = 0.33f;
                var modifier = Mathf.Max(1, animTime / cooldown);
                _warrior.Animation.SetAttackSpeed(modifier);
                _warrior.ApplyAttack(_warrior.AttackTarget);
                _counter = 1f / _warrior.Model.AttackSpeed;
            }   
        }
    }
}