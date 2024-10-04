using Zenject;

namespace Core
{
    public class GameLoopInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameLoop>()
                .AsSingle();
        }
    }
}