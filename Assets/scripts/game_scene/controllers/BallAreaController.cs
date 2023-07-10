using System.Linq;
using game_scene.models;
using game_scene.views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace game_scene.controllers {
public class BallAreaController : MonoBehaviour {
    [Inject] BallAreaView view;
    [Inject] ArtView artView;
    [Inject] BallFactory ballFactory;
    [Inject(Id = UiElementId.TestLabel)] TMP_Text testLabel;
    [Inject(Id = UiElementId.ShuffleButton)] Button shuffleButton;

    Log log;
    Ball[,] balls;
    GameObject ballContainer;
    Vector2Int artSize;
    bool[] rowsSolved;
    bool artSolved;

    void Awake() {
        log = new(GetType());
        ballContainer = new("Balls");
    }

    void Start() {
        artSize = artView.getArtSizeInPixels();
        balls = new Ball[artSize.x, artSize.y];
        rowsSolved = new bool[artSize.y];
        createBalls();
        randomizeRows();
        testLabel.text = "";
        shuffleButton.onClick.AddListener(randomizeRows);
    }

    void createBalls() {
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

    void randomizeRows() {
        var halfWidth = artSize.x / 2;
        for (var rowIndex = 0; rowIndex < artSize.y; rowIndex++) {
            var shiftCount = RandomUtils.nextInt(1, halfWidth);
            var left = RandomUtils.nextBool();
            for (var j = 0; j < shiftCount; j++) {
                shiftRow(rowIndex, left);
            }
        }
    }

    public void shiftRow(int rowIndex, bool left) {
        shiftBallsInArray(rowIndex, left);
        var y = rowIndex;
        for (var x = 0; x < artSize.x; x++) {
            var ball = balls[x, y];
            ball.setPosition(view.getBallShiftPosition(x, y), x, y);
        }
    }

    void shiftBallsInArray(int rowIndex, bool left) {
        var y = rowIndex;
        if (left) {
            var first = balls[0, y];
            for (var i = 0; i < artSize.x - 1; i++) {
                balls[i, y] = balls[i + 1, y];
            }
            balls[artSize.x - 1, y] = first;
        } else {
            var last = balls[artSize.x - 1, y];
            for (var i = artSize.x - 1; i > 0; i--) {
                balls[i, y] = balls[i - 1, y];
            }
            balls[0, y] = last;
        }
    }

    public void checkIfComplete(int rowIndex) {
        checkRow(rowIndex);
        artSolved = rowsSolved.All(rowSolved => rowSolved);
        testLabel.text = artSolved ? "art is solved" : "solving..";
        if (artSolved) {
            log.log("art is solved!");
        }
    }

    void checkRow(int rowIndex) {
        var y = rowIndex;
        var complete = true;
        for (var x = 0; x < artSize.x; x++) {
            if (balls[x, y].color != artView.getPixelColor(x, y)) {
                complete = false;
                break;
            }
        }
        rowsSolved[y] = complete;
    }
}
}