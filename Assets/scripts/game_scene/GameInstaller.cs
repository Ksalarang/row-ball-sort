using game_scene.controllers;
using game_scene.views;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// ReSharper disable All

namespace game_scene {
public class GameInstaller : MonoInstaller {
    [SerializeField] GameController gameController;
    [Header("Controllers")]
    [SerializeField] BallFactory ballFactory;
    [SerializeField] BallAreaController ballAreaController;
    [SerializeField] BallInputController ballInputController;
    [Header("Views")]
    [SerializeField] ArtView artView;
    [SerializeField] BallAreaView ballAreaView;
    [Header("Prefabs")]
    [SerializeField] GameObject ballPrefab;
    [Header("Misc")] [SerializeField] new Camera camera;
    [SerializeField] CanvasScaler canvasScaler;

    public override void InstallBindings() {
        bind(gameController);
        // controllers
        bind(ballFactory);
        bindAsSingle<CanvasController>();
        bind(ballAreaController);
        bind(ballInputController);
        // views
        bind(artView);
        bind(ballAreaView);
        // prefabs
        bind(ballPrefab, PrefabId.Ball);
        // misc
        bind(camera);
        bind(canvasScaler);
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

public enum PrefabId {
    Ball,
}
}
