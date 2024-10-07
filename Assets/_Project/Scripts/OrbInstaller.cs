using Audio;
using Zenject;

public class OrbInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DesaturationMaskView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Orb>().FromComponentInHierarchy().AsSingle();
    }
}
