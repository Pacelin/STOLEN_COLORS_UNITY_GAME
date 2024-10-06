using Zenject;

namespace Gameplay.Map.Bosses
{
    public class BossReference
    {
        public bool BossIsAlive => _boss && _boss.Model.Alive.Value;
        public Boss Boss => _boss;

        [Inject] private Boss _boss;
    }
}