namespace Gameplay.Map
{
    public class WarriorWaitSignalState : WarriorState
    {
        public override void Enter()
        {
            _warrior.Agent.stoppingDistance = 0;
            _warrior.Agent.SetDestination(_warrior.SnapPosition);
            _warrior.Animation.SetIdle();
        }

        public override void Exit()
        {
            if (_warrior.Agent.isOnNavMesh && _warrior.Agent.isActiveAndEnabled)
                _warrior.Agent.ResetPath();
        }
    }
}