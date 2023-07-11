using System.Linq;
using System.Text;
using game_scene.models;
using game_scene.views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.Extensions;
using utils.structs;
using Zenject;

namespace game_scene.controllers {
public class BallAreaController : MonoBehaviour {
    [Inject] BallAreaView view;
    [Inject] ArtView artView;
    [Inject] BallFactory ballFactory;
    [Inject(Id = UiElementId.TestLabel)] TMP_Text testLabel;
    [Inject(Id = UiElementId.ShuffleButton)] Button shuffleButton;
    [Inject] ArtScrambleSettings artScrambleSettings;

    Log log;
    Ball[,] balls;
    Vector2Int[] coords;
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
        initCoords();
        rowsSolved = new bool[artSize.y];
        createBalls();
        testLabel.text = "";
        scrambleBalls();
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

    void initCoords() {
        coords = new Vector2Int[artSize.x * artSize.y];
        var i = 0;
        for (var x = 0; x < artSize.x; x++) {
            for (var y = 0; y < artSize.y; y++) {
                coords[i] = new Vector2Int(x, y);
                i++;
            }
        }
    }

    void scrambleRows() {
        var halfWidth = artSize.x / 2;
        for (var rowIndex = 0; rowIndex < artSize.y; rowIndex++) {
            var shiftCount = RandomUtils.nextInt(1, halfWidth);
            var left = RandomUtils.nextBool();
            for (var j = 0; j < shiftCount; j++) {
                shiftRow(rowIndex, left);
            }
        }
    }

    void scrambleBalls() {
        var scrambleAmount = (int) (artSize.x * artSize.y * artScrambleSettings.scramblePercentage);
        coords.shuffle();
        for (var i = 0; i < scrambleAmount; i++) {
            var coord = coords[i];
            var direction = randomDirection(coord);
            var nextCoord = coord + direction.toVector();
            swapBalls(coord, nextCoord);
        }
    }

    void swapBalls(Vector2Int coord1, Vector2Int coord2) {
        var first = balls[coord1.x, coord1.y];
        var second = balls[coord2.x, coord2.y];
        balls[coord1.x, coord1.y] = second;
        balls[coord2.x, coord2.y] = first;
        first.setPosition(view.getBallPosition(coord2), coord2.x, coord2.y);
        second.setPosition(view.getBallPosition(coord1), coord1.x, coord1.y);
    }

    Direction randomDirection(Vector2Int ballPosition) {
        var xRange = new IntRange(0, artSize.x - 1);
        var yRange = new IntRange(0, artSize.y - 1);
        Direction direction;
        Vector2Int vector;
        do {
            direction = RandomUtils.nextEnum<Direction>();
            vector = direction.toVector();
            vector += ballPosition;
        } while (!xRange.includes(vector.x) || !yRange.includes(vector.y));
        return direction;
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