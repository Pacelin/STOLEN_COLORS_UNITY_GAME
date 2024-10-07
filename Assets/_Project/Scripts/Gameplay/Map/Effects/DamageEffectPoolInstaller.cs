using Zenject;

namespace _Project.Scripts.Gameplay.Map.Effects
{
    public class DamageEffectPoolInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DamageEffectPool>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}