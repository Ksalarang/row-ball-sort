using services.sounds;
using UnityEngine;
using UnityEngine.UI;
using Utils.MVC;
using Zenject;

namespace game_scene.windows {
public class SettingsView : View {
    [SerializeField] Button closeButton;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;

    [Inject] SoundService soundService;

    void Awake() {
        closeButton.onClick.AddListener(animateHide);
        soundSlider.onValueChanged.AddListener(onSoundVolumeChanged);
        musicSlider.onValueChanged.AddListener(onMusicVolumeChanged);
        soundSlider.value = soundService.getSoundVolume();
        musicSlider.value = soundService.getMusicVolume();
    }

    void onSoundVolumeChanged(float value) {
        soundService.setSoundVolume(value);
    }

    void onMusicVolumeChanged(float value) {
        soundService.setMusicVolume(value);
    }

    public void animateShow() {
        show(FadeDuration);
    }

    public void animateHide() {
        hide(FadeDuration);
    }
}
}