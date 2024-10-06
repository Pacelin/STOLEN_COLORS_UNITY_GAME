using UnityEngine;
using Zenject;

namespace Gameplay.Map.Spawn
{
    public class WarriorsInstaller : MonoInstaller
    {
        [SerializeField] private WarriorsConfig _warriors;

        public override void InstallBindings()
        {
            Container.Bind<WarriorsConfig>()
                .FromScriptableObject(_warriors)
                .AsSingle();
            Container.Bind<WarriorsSpawner>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<WarriorsCollection>()
                .AsSingle();
            Container.Bind<WaveManager>()
                .AsSingle();
        }
    }
}