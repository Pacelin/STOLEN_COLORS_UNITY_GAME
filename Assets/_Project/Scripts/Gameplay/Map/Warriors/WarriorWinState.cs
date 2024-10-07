using Audio;
using Zenject;

namespace Gameplay.Map
{
    public class WarriorWinState : WarriorState
    {
        [Inject]
        private DesaturationMaskController _maskController;
        [Inject]
        private Orb _orb;
        
        public override void Enter()
        {
            _warrior.Agent.SetDestination(_orb.transform.position);
            _warrior.Animation.SetWalk();
        }

        public override void Exit()
        {
            _warrior.Animation.SetIdle();
        }
    }
}