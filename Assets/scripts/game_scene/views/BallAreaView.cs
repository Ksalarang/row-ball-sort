﻿using game_scene.models;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;
using utils.structs;
using Zenject;

namespace game_scene.views {
public class BallAreaView : MonoBehaviour {
    [SerializeField] Image ballAreaImage;

    [Inject] ArtView artView;
    [Inject] BallAreaSettings settings;

    Vector2Int artSize;
    Rect ballAreaRect;
    Vector3 ballSize;
    Vector3 bottomLeft;

    public float step { get; private set; }
    public FloatRange widthRange { get; private set; }
    
    public void onArtDataReady() {
        artSize = artView.getArtSizeInPixels();
        ballAreaRect = ballAreaImage.rectTransform.getWorldRect();
        var ballWidth = (ballAreaRect.width - artSize.x * settings.distanceBetweenBalls) / artSize.x;
        ballSize = new Vector3(ballWidth, ballWidth);
        step = ballWidth + settings.distanceBetweenBalls;
        var offset = settings.distanceBetweenBalls / 2 + ballWidth / 2;
        bottomLeft = new Vector3(ballAreaRect.x + offset, ballAreaRect.y + offset);
        widthRange = new FloatRange(ballAreaRect.xMin, ballAreaRect.xMax);
    }
    
    public Vector3 getBallSize() => ballSize;

    public Vector3 getBallPosition(Vector2Int coord) => getBallPosition(coord.x, coord.y);
    
    public Vector3 getBallPosition(int x, int y) {
        return new Vector3(
            bottomLeft.x + x * step,
            bottomLeft.y + y * step
        );
    }

    public Vector3 getBallShiftPosition(Vector2Int coord) => getBallShiftPosition(coord.x, coord.y);

    public Vector3 getBallShiftPosition(int x, int y) {
        if (x < 0) x = artSize.x - 1;
        else if (x >= artSize.x) x = 0;
        if (y < 0) y = artSize.y - 1;
        else if (y >= artSize.y) y = 0;
        return getBallPosition(x, y);
    }

    public FloatRange getHeightRange() => new(ballAreaRect.yMin, ballAreaRect.yMax);

    public float getRowHeight() => ballSize.x + settings.distanceBetweenBalls;
}
}