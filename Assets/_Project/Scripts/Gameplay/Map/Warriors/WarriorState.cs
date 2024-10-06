using Zenject;

namespace Gameplay.Map
{
    public abstract class WarriorState
    {
        [Inject] protected Warrior _warrior;
        
        public abstract void Enter();
        public abstract void Exit();
    }
}