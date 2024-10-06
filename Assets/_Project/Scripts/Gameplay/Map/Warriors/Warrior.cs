using System;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class Warrior : Unit
    {
        public EWarriorClass Class => _class;
        public Vector3 SnapPosition => _snapPosition;
        public Warrior AttackTarget => _attackTarget;
        
        [SerializeField] private EWarriorClass _class;

        private WarriorStateMachine _stateMachine;
        private Warrior _attackTarget;
        private Vector3 _snapPosition;

        [Inject]
        private void Construct(DiContainer container)
        {
            Agent.speed = Model.Speed;
            _stateMachine = new WarriorStateMachine(container, this);
        }

        public void Release() => _stateMachine.SwitchState<WarriorWalkToCastleState>();

        public void Snap(Vector3 position)
        {
            _snapPosition = position;
            _stateMachine.SwitchState<WarriorWaitSignalState>();
        }

        public void SetAttack(Warrior warrior)
        {
            _attackTarget = warrior;
            _stateMachine.SwitchState<WarriorAttackEnemyState>();
        }
        
        private void OnEnable() =>
            _stateMachine.Run();
        private void OnDisable() => 
            _stateMachine.Stop();
    }
}