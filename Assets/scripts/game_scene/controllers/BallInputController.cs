using game_scene.views;
using UnityEngine;
using Utils;
using Zenject;

namespace game_scene.controllers {
public class BallInputController : MonoBehaviour {
    [SerializeField] float threshold;
    
    [Inject] new Camera camera;
    [Inject] BallAreaView ballAreaView;
    [Inject] BallAreaController ballAreaController;

    Log log;
    bool isMobile;
    FloatRange heightRange;
    float rowHeight;
    float verticalOffset;
    
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
    }

    void Update() {
        if (isMobile) {
            if (Input.touchCount == 0) return;
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
        switch (phase) { 
            case TouchPhase.Began:
                startPosition = camera.ScreenToWorldPoint(Input.mousePosition);
                startedTouchInArea = heightRange.min < startPosition.y && startPosition.y < heightRange.max;
                if (!startedTouchInArea) return;
                rowIndex = (int) ((startPosition.y + verticalOffset) / rowHeight);
                log.log($"start touch at row {rowIndex}");
                break;
            case TouchPhase.Moved:
                if (startedTouchInArea) {
                    var delta = startPosition.x - currentPosition.x;
                    if (Mathf.Abs(delta) > threshold) {
                        var left = delta > 0;
                        ballAreaController.shiftRow(rowIndex, left);
                        startPosition = currentPosition;
                        log.log($"shift row {rowIndex} " + (left ? "left" : "right"));
                    }
                }
                break;
            case TouchPhase.Ended:
                if (startedTouchInArea) {
                    log.log($"end touch");
                }
                break;
        }
    }
}
}