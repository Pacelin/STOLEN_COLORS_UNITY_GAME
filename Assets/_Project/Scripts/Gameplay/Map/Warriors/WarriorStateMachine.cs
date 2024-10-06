using System;
using System.Collections.Generic;
using Zenject;

namespace Gameplay.Map
{
    public class WarriorStateMachine
    {
        private readonly Dictionary<Type, WarriorState> _states;
        private WarriorState _currentState;
        private bool _isRun;
        
        public WarriorStateMachine(DiContainer container, Warrior warrior)
        {
            var args = new object[] { warrior };
            _states = new Dictionary<Type, WarriorState>()
            {
                { typeof(WarriorWaitSignalState), container.Instantiate<WarriorWaitSignalState>(args) },
                { typeof(WarriorWalkToCastleState), container.Instantiate<WarriorWalkToCastleState>(args) },
                { typeof(WarriorAttackEnemyState), container.Instantiate<WarriorAttackEnemyState>(args) },
                { typeof(WarriorWinState), container.Instantiate<WarriorWinState>(args) }
            };
            _isRun = false;
        }

        public void Run()
        {
            if (_isRun) return;
            if (_currentState == null)
                _currentState = _states[typeof(WarriorWaitSignalState)];
            _currentState.Enter();
            _isRun = true;
        }

        public void Stop()
        {
            if (!_isRun) return;
            _currentState.Exit();
            _currentState = null;
            _isRun = false;
        }
        
        public void SwitchState<T>() where T : WarriorState
        {
            _isRun = true;
            _currentState?.Exit();
            _currentState = _states[typeof(T)];
            _currentState.Enter();
        }

        public void Update()
        {
            if (_isRun)
                _currentState?.Update();
        }

        public void FixedUpdate()
        {
            if (_isRun)
                _currentState?.FixedUpdate();
        }
    }
}