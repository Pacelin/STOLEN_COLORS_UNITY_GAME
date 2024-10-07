using UnityEngine;

namespace Gameplay.Map
{
    public class WarriorAttackEnemyState : WarriorState
    {
        private const float UPDATE_DESTINATION_THRESHOLD = 0.1f;
        private float _counter;
        
        public override void Enter()
        {
            _warrior.Agent.stoppingDistance = 
                _warrior.Model.AttackDistance + _warrior.Agent.radius + _warrior.AttackTarget.Agent.radius;
            _counter = 0;
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
        }
        
        private void AttackFixedUpdate()
        {
            if (_counter <= 0)
            {
                _warrior.ApplyAttack(_warrior.AttackTarget);
                _counter = 1f / _warrior.Model.AttackSpeed;
            }   
        }
    }
}