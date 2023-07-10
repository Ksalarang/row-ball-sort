using game_scene.controllers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using Zenject;

namespace game_scene.views {
public class BallAreaView : MonoBehaviour {
    [SerializeField] Image ballAreaImage;
    [SerializeField] float distanceBetweenBalls;

    [Inject] ArtView artView;
    [Inject] CanvasController canvasController;
    [Inject] BallAreaController controller;

    Vector2Int artSize;
    Rect ballAreaRect;
    Vector3 ballSize;
    float step;
    Vector3 bottomLeft;
    
    public void onArtDataReady() {
        artSize = artView.getArtSizeInPixels();
        ballAreaRect = ballAreaImage.rectTransform.getWorldRect(canvasController.getCanvasMinScale());
        var ballWidth = (ballAreaRect.width - (artSize.x + 1) * distanceBetweenBalls) / artSize.x;
        ballSize = new Vector3(ballWidth, ballWidth);
        step = ballWidth + distanceBetweenBalls;
        var offset = distanceBetweenBalls + ballWidth / 2;
        bottomLeft = new Vector3(ballAreaRect.x + offset, ballAreaRect.y + offset);
    }
    
    public Vector3 getBallSize() => ballSize;

    public Vector3 getBallPosition(int x, int y) {
        return new Vector3(
            bottomLeft.x + x * step,
            bottomLeft.y + y * step
        );
    }

    public Vector3 getBallShiftPosition(int x, int y) {
        if (x < 0) x = artSize.x - 1;
        else if (x >= artSize.x) x = 0;
        if (y < 0) y = artSize.y - 1;
        else if (y >= artSize.y) y = 0;
        return getBallPosition(x, y);
    }

    public FloatRange getHeightRange() => new(ballAreaRect.yMin, ballAreaRect.yMax);

    public float getRowHeight() => ballSize.x + distanceBetweenBalls;
}
}