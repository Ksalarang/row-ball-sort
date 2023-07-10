using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace game_scene.controllers {
public class CanvasController {
    readonly CanvasScaler canvasScaler;

    [Inject]
    public CanvasController(CanvasScaler canvasScaler) {
        this.canvasScaler = canvasScaler;
    }

    public float getCanvasWidthScale() => Screen.width / canvasScaler.referenceResolution.x;
    
    public float getCanvasHeightScale() => Screen.height / canvasScaler.referenceResolution.y;

    public float getCanvasMinScale() => Mathf.Min(getCanvasWidthScale(), getCanvasHeightScale());
}
}