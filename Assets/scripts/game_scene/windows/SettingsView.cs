using UnityEngine;
using UnityEngine.UI;
using Utils.MVC;

namespace game_scene.windows {
public class SettingsView : View {
    [SerializeField] Button closeButton;

    void Awake() {
        closeButton.onClick.AddListener(animateHide);
    }

    public void animateShow() {
        show(FadeDuration);
    }

    public void animateHide() {
        hide(FadeDuration);
    }
}
}