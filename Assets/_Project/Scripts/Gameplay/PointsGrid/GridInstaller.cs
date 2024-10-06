using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GridPanel>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}