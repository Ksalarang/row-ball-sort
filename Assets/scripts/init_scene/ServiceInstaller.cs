using services.scenes;
using services.sounds;
using UnityEngine;
using Zenject;

// ReSharper disable All

namespace init_scene {
public class ServiceInstaller : MonoInstaller {
    [SerializeField] AudioSources audioSources;
    
    public override void InstallBindings() {
        bind(audioSources);
        // services
        bind<SceneService, SimpleSceneService>();
        bind<SoundService>();
    }

    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }

    void bind<T>() {
        Container.Bind<T>().FromNew().AsSingle().NonLazy();
    }

    void bind<Interface, Implementation>() where Implementation : Interface {
        Container.Bind<Interface>().To<Implementation>().AsSingle().NonLazy();
    }
}
}