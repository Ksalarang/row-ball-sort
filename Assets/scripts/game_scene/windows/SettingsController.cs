using Utils.MVC;
using Zenject;

namespace game_scene.windows {
public class SettingsController : Controller {
    [Inject] SettingsView view;

    public void show() {
        view.animateShow();
    }
}
}