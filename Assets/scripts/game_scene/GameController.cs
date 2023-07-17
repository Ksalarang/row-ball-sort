using game_scene.controllers;
using game_scene.windows;
using services.sounds;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace game_scene {
public class GameController : MonoBehaviour {
    [Inject] SoundService soundService;
    [Inject(Id = UiElementId.SettingsButton)] Button settingsButton;
    [Inject] SettingsController settingsController;
    [Inject] BallInputController ballInputController;
    
    Log log;

    void Awake() {
        log = new(GetType());
        Application.targetFrameRate = 60;
        settingsButton.onClick.AddListener(() => {
            ballInputController.paused = true;
            settingsController.setOnHideAction(() => ballInputController.paused = false);
            settingsController.show();
        });
    }

    void Start() {
        log.log("start");
        soundService.playSoundtrack(SoundTrackId.Background);
    }
}
}