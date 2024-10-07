using Audio;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class WarriorWinState : WarriorState
    {
        [Inject]
        private DesaturationMaskView _view;
        [Inject]
        private Orb _orb;

        private float distance = 5f;
        
        public override void Enter()
        {
            _warrior.Agent.stoppingDistance = distance / 2f;
            _warrior.Agent.SetDestination(_orb.transform.position);
            _warrior.Animation.SetWalk();
        }

        public override void Update()
        {
            if (_warrior.Agent.remainingDistance < distance)
            {
                _warrior.Animation.SetIdle();
                _view.ExpandPrism();
            }
        }

        public override void Exit()
        {
            _warrior.Animation.SetIdle();
        }
    }
}