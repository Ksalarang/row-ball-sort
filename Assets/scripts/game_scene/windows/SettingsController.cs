using System;
using Utils.MVC;
using Zenject;

namespace game_scene.windows {
public class SettingsController : Controller {
    [Inject] SettingsView view;

    public void show() {
        view.animateShow();
    }

    public void setOnHideAction(Action action) {
        view.onHideAction = action;
    }
}
}