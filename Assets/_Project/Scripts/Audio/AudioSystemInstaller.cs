using UnityEngine;
using Zenject;

namespace Audio
{
    public class AudioSystemInstaller : MonoInstaller
    {
        [SerializeField] private AudioSystem _audioSystemPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AudioSystem>()
                .FromComponentInNewPrefab(_audioSystemPrefab)
                .WithGameObjectName("Audio System")
                .AsSingle()
                .OnInstantiated<AudioSystem>((ic, o) => GameObject.DontDestroyOnLoad(o))
                .NonLazy();
        }
    }
}