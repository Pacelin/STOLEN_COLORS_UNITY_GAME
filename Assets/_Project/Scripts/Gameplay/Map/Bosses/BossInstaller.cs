using UnityEngine;
using Zenject;

namespace Gameplay.Map.Bosses
{
    public class BossInstaller : MonoInstaller
    {
        [SerializeField] private Boss _bossPrefab;

        public override void InstallBindings()
        {
            Container.Bind<Boss>()
                .FromComponentInNewPrefab(_bossPrefab)
                .AsSingle()
                .NonLazy();
            Container.Bind<BossReference>()
                .AsSingle();
        }
    }
}