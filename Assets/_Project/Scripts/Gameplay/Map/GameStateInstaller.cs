using Zenject;

namespace Gameplay.Map
{
    public class GameStateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStateManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("Game State Manager")
                .AsSingle();
        }
    }
}