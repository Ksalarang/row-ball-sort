using services.sounds;
using services.vibrations;
using UnityEngine;
using UnityEngine.UI;
using Utils.MVC;
using Zenject;

namespace game_scene.windows {
public class SettingsView : View {
    [SerializeField] Button closeButton;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider vibrationSlider;
    [SerializeField] GameObject vibrationContainer;

    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;

    void Awake() {
        closeButton.onClick.AddListener(animateHide);
        soundSlider.onValueChanged.AddListener(onSoundVolumeChanged);
        musicSlider.onValueChanged.AddListener(onMusicVolumeChanged);
        soundSlider.value = soundService.getSoundVolume();
        musicSlider.value = soundService.getMusicVolume();
        if (vibrationService.supportsVibration()) {
            vibrationSlider.onValueChanged.AddListener(onVibrationValueChanged);
            vibrationSlider.value = vibrationService.isVibrationEnabled() ? 1 : 0;
        } else {
            vibrationContainer.SetActive(false);
        }
    }

    void onSoundVolumeChanged(float value) {
        soundService.setSoundVolume(value);
    }

    void onMusicVolumeChanged(float value) {
        soundService.setMusicVolume(value);
    }

    void onVibrationValueChanged(float value) {
        if (value == 1) {
            vibrationService.setVibrationEnabled(true);
            vibrationService.vibrate(VibrationType.Light);
        } else {
            vibrationService.setVibrationEnabled(false);
        }
    }

    public void animateShow() {
        show(FadeDuration);
    }

    public void animateHide() {
        hide(FadeDuration);
    }
}
}