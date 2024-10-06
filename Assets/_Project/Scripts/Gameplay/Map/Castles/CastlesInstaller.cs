using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class CastlesInstaller : MonoInstaller
    {
        [SerializeField] private CastlesCollection _prefab;

        public override void InstallBindings()
        {
            Container.Bind<CastlesCollection>()
                .FromComponentInNewPrefab(_prefab)
                .WithGameObjectName("Castles")
                .AsSingle()
                .NonLazy();
        }
    }
}