using game_scene.models;
using game_scene.views;
using UnityEngine;
using Utils;
using Zenject;

namespace game_scene.controllers {
public class BallAreaController : MonoBehaviour {
    [Inject] BallAreaView view;
    [Inject] ArtView artView;
    [Inject] BallFactory ballFactory;

    Log log;
    Ball[,] balls;
    GameObject ballContainer;

    void Awake() {
        log = new(GetType());
        ballContainer = new("Balls");
    }

    void Start() {
        var size = artView.getArtSizeInPixels();
        balls = new Ball[size.x, size.y];
        createBalls();
    }

    void createBalls() {
        var artSize = artView.getArtSizeInPixels();
        for (var x = 0; x < artSize.x; x++) {
            for (var y = 0; y < artSize.y; y++) {
                var ball = ballFactory.create();
                var ballTransform = ball.transform;
                ballTransform.SetParent(ballContainer.transform);
                ballTransform.localScale = view.getBallSize();
                ball.setPosition(view.getBallPosition(x, y), x, y);
                ball.color = artView.getPixelColor(x, y);
                balls[x,y] = ball;
            }
        }
    }

    public void shiftRow(int rowIndex, bool left) {
        shiftBallsInArray(rowIndex, left);
        var size = artView.getArtSizeInPixels();
        var y = rowIndex;
        for (var x = 0; x < size.x; x++) {
            var ball = balls[x, y];
            ball.setPosition(view.getBallShiftPosition(x, y), x, y);
        }
    }

    void shiftBallsInArray(int rowIndex, bool left) {
        var size = artView.getArtSizeInPixels();
        var y = rowIndex;
        if (left) {
            var first = balls[0, y];
            for (var i = 0; i < size.x - 1; i++) {
                balls[i, y] = balls[i + 1, y];
            }
            balls[size.x - 1, y] = first;
        } else {
            var last = balls[size.x - 1, y];
            for (var i = size.x - 1; i > 0; i--) {
                balls[i, y] = balls[i - 1, y];
            }
            balls[0, y] = last;
        }
    }
}
}