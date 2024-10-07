using Audio.Gameplay;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class GameStateInstaller : MonoInstaller
    {
        [SerializeField] private CanvasGroup _loseCanvas;
        
        public override void InstallBindings()
        {
            Container.Bind<GameStateManager>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("Game State Manager")
                .AsSingle();
            Container.BindInterfacesAndSelfTo<LoseState>()
                .AsSingle()
                .WithArguments(_loseCanvas);
        }
    }
}