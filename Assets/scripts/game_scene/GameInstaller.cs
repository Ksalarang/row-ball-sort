using UnityEngine;
using Zenject;

// ReSharper disable All

namespace game_scene {
public class GameInstaller : MonoInstaller {
    [SerializeField] GameController gameController;

    public override void InstallBindings() {
        bind(gameController);
    }
    
    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }

    void bind<T>(T instance, object id) {
        Container.Bind<T>().WithId(id).FromInstance(instance);
    }

    void bindAsSingle<T>() {
        Container.Bind<T>().FromNew().AsSingle();
    }

    void bindWithInterfaces<T>(T instance) {
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance);
    }
}
}
