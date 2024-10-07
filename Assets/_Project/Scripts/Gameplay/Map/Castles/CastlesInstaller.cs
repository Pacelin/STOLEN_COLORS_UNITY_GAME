using Zenject;

namespace Gameplay.Map
{
    public class CastlesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<CastlesCollection>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}