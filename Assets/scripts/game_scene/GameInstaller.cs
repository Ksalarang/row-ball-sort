using game_scene.controllers;
using game_scene.models;
using game_scene.views;
using TMPro;
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
    [Header("UI elements")]
    [SerializeField] TMP_Text testLabel;
    [SerializeField] Button shuffleButton;
    [SerializeField] Button resetButton;
    [Header("Prefabs")]
    [SerializeField] GameObject ballPrefab;
    [Header("Misc")]
    [SerializeField] new Camera camera;
    [SerializeField] CanvasScaler canvasScaler;
    [SerializeField] GameSettings gameSettings;

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
        // UI elements
        bind(testLabel, UiElementId.TestLabel);
        bind(shuffleButton, UiElementId.ShuffleButton);
        bind(resetButton, UiElementId.ResetButton);
        // prefabs
        bind(ballPrefab, PrefabId.Ball);
        // misc
        bind(camera);
        bind(canvasScaler);
        // settings
        bind(gameSettings);
        bind(gameSettings.artScrambleSettings);
        bind(gameSettings.inputSettings);
        bind(gameSettings.ballAreaSettings);
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

public enum UiElementId {
    TestLabel,
    ShuffleButton,
    ResetButton,
}

public enum PrefabId {
    Ball,
}
}
