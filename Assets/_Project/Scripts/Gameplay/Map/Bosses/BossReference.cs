using System;
using Zenject;

namespace Gameplay.Map.Bosses
{
    public class BossReference
    {
        public bool BossIsAlive => _boss && _boss.Model.Alive.Value;
        public Boss Boss => _boss;
        public IObservable<UniRx.Unit> OnBossActivated => _boss.OnActivate; 

        [Inject] private Boss _boss;
    }
}