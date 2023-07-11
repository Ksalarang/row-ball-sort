using game_scene.models;
using game_scene.views;
using UnityEngine;
using Utils;
using utils.structs;
using Zenject;

namespace game_scene.controllers {
public class BallInputController : MonoBehaviour {
    [Inject] new Camera camera;
    [Inject] BallAreaView ballAreaView;
    [Inject] BallAreaController ballAreaController;
    [Inject] ArtView artView;
    [Inject] InputSettings settings;

    Log log;
    bool isMobile;
    FloatRange heightRange;
    float rowHeight;
    float verticalOffset;
    Vector2Int artSize;
    
    Vector3 startPosition;
    int rowIndex;
    bool startedTouchInArea;
    // for mouse input handling
    bool touchedBefore;
    bool touching;
    TouchPhase phase;
    Vector3 prevPosition;
    Vector3 currentPosition;

    void Awake() {
        log = new(GetType(), false);
        isMobile = Application.isMobilePlatform;
    }

    void Start() {
        heightRange = ballAreaView.getHeightRange();
        rowHeight = ballAreaView.getRowHeight();
        verticalOffset = Mathf.Abs(heightRange.min);
        artSize = artView.getArtSizeInPixels();
    }

    bool swipeDirectionDetermined;
    bool swipeProcessed;
    bool vertical;

    void Update() {
        #region determine touch phase
        if (isMobile) {
            if (Input.touchCount != 1) return;
            var touch = Input.GetTouch(0);
            phase = touch.phase;
            currentPosition = camera.ScreenToWorldPoint(touch.position);
        } else {
            touching = Input.GetMouseButton(0);
            currentPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            if (!touchedBefore && touching) {
                phase = TouchPhase.Began;
                touchedBefore = true;
                prevPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            } else if (touchedBefore && touching) {
                if (prevPosition.x - currentPosition.x != 0 || prevPosition.y - currentPosition.y != 0) {
                    phase = TouchPhase.Moved;
                    prevPosition = currentPosition;
                } else {
                    phase = TouchPhase.Stationary;
                }
            } else if (touchedBefore && !touching) {
                phase = TouchPhase.Ended;
                touchedBefore = false;
            } else return;
        }
        #endregion
        switch (phase) {
            case TouchPhase.Began:
                startPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                startedTouchInArea = heightRange.min < startPosition.y && startPosition.y < heightRange.max;
                if (!startedTouchInArea) return;
                rowIndex = (int) ((startPosition.y + verticalOffset) / rowHeight);
                rowIndex = Mathf.Clamp(rowIndex, 0, artSize.y - 1);
                log.log($"start touch at row {rowIndex}");
                break;
            case TouchPhase.Moved:
                if (startedTouchInArea) {
                    var deltaX = startPosition.x - currentPosition.x;
                    var deltaY = startPosition.y - currentPosition.y;
                    if (!swipeDirectionDetermined) {
                        vertical = Mathf.Abs(deltaY) > Mathf.Abs(deltaX);
                        swipeDirectionDetermined = true;
                    }
                    switch (vertical) {
                        case true when !swipeProcessed && Mathf.Abs(deltaY) > 1 - settings.verticalSwipeSensitivity: {
                            var up = deltaY < 0;
                            ballAreaController.onSwipe(up, rowIndex, startPosition);
                            startPosition = currentPosition;
                            swipeProcessed = true;
                            log.log($"swipe " + (up ? "up" : "down"));
                            break;
                        }
                        case false when Mathf.Abs(deltaX) > 1 - settings.horizontalSwipeSensitivity: {
                            var left = deltaX > 0;
                            ballAreaController.shiftRow(rowIndex, left);
                            startPosition = currentPosition;
                            log.log($"shift row {rowIndex} " + (left ? "left" : "right"));
                            break;
                        }
                    }
                }
                break;
            case TouchPhase.Ended:
                if (startedTouchInArea) {
                    ballAreaController.checkIfComplete(rowIndex);
                    swipeDirectionDetermined = false;
                    swipeProcessed = false;
                    log.log($"end touch");
                }
                break;
        }
    }
}
}