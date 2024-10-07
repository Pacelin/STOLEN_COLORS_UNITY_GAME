using Audio;
using UnityEngine;
using Zenject;

public class OrbInstaller : MonoInstaller
{
    [SerializeField]
    private DesaturationMaskController _maskController;
    [SerializeField]
    private Orb _orb;
    
    public override void InstallBindings()
    {
        Container.Bind<DesaturationMaskController>().FromInstance(_maskController).AsSingle();
        Container.Bind<Orb>().FromInstance(_orb).AsSingle();
    }
}
