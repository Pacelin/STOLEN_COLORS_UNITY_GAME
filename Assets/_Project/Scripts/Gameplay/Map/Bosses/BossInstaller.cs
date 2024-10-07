using Zenject;

namespace Gameplay.Map.Bosses
{
    public class BossInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Boss>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
            Container.Bind<BossReference>()
                .AsSingle();
        }
    }
}