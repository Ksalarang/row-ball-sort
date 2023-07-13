using System;
using System.Linq;
using game_scene.models;
using game_scene.views;
using services.sounds;
using services.vibrations;
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
    [Inject(Id = UiElementId.ResetButton)] Button resetButton;
    [Inject] ArtScrambleSettings artScrambleSettings;
    [Inject] BallAnimationSettings animationSettings;
    [Inject] BallAreaSettings settings;
    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;

    Log log;
    GameObject ballContainer;
    Ball[,] balls;
    Ball[,] ballsCopy;
    Ball leftEdgeBall;
    Ball rightEdgeBall;
    Vector2Int[] coords;
    Vector2Int artSize;
    bool[] rowsSolved;
    bool artSolved;

    #region
    void Awake() {
        log = new(GetType(), true);
        ballContainer = new("Balls");
        shuffleButton.onClick.AddListener(scrambleBalls);
        resetButton.onClick.AddListener(resetBalls);
    }

    void Start() {
        artSize = artView.getArtSizeInPixels();
        initCoords();
        rowsSolved = new bool[artSize.y];
        createBalls();
        scrambleBalls();
        testLabel.text = "solve the art";
    }

    void createBalls() {
        balls = new Ball[artSize.x, artSize.y];
        ballsCopy = new Ball[artSize.x, artSize.y];
        for (var x = 0; x < artSize.x; x++) {
            for (var y = 0; y < artSize.y; y++) {
                var ball = ballFactory.create($"ball ({x},{y})");
                var ballTransform = ball.transform;
                ballTransform.SetParent(ballContainer.transform);
                ballTransform.localScale = view.getBallSize();
                var gridPosition = new Vector2Int(x, y);
                ball.setPosition(view.getBallPosition(gridPosition), gridPosition);
                ball.initialPosition = gridPosition;
                ball.color = artView.getPixelColor(x, y);
                balls[x, y] = ball;
                ballsCopy[x, y] = ball;
            }
        }
        createEdgeBalls();
        for (var i = 0; i < rowsSolved.Length; i++) rowsSolved[i] = true;
    }

    void createEdgeBalls() {
        leftEdgeBall = ballFactory.create("leftEdgeBall");
        var leftTransform = leftEdgeBall.transform;
        leftTransform.SetParent(ballContainer.transform);
        leftTransform.setX(view.widthRange.max + 10 * view.step);
        leftTransform.localScale = view.getBallSize();
        rightEdgeBall = ballFactory.create("rightEdgeBall");
        var rightTransform = rightEdgeBall.transform;
        rightTransform.SetParent(ballContainer.transform);
        rightTransform.setX(view.widthRange.min - 10 * view.step);
        rightTransform.localScale = view.getBallSize();
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
    #endregion

    #region click listeners
    void scrambleRows() {
        var halfWidth = artSize.x / 2;
        for (var rowIndex = 0; rowIndex < artSize.y; rowIndex++) {
            var shiftCount = RandomUtils.nextInt(1, halfWidth);
            var left = RandomUtils.nextBool();
            for (var j = 0; j < shiftCount; j++) {
                shiftRow(rowIndex, left);
            }
            rowsSolved[rowIndex] = false;
        }
    }
    
    void shiftRow(int rowIndex, bool left) {
        shiftBallsInArray(rowIndex, left);
        var y = rowIndex;
        for (var x = 0; x < artSize.x; x++) {
            var ball = balls[x, y];
            ball.setPosition(view.getBallShiftPosition(x, y), x, y);
        }
    }

    void scrambleBalls() {
        var scrambleAmount = (int) (artSize.x * artSize.y * artScrambleSettings.scramblePercentage);
        coords.shuffle(); //todo: optimize
        for (var i = 0; i < scrambleAmount; i++) {
            var coord = coords[i];
            var nextCoord = randomCoordinate(coord);
            if (balls[coord.x, coord.y].color.approximately(balls[nextCoord.x, nextCoord.y].color)) {
                scrambleAmount++;
                continue;
            }
            swapBalls(coord, nextCoord);
            rowsSolved[coord.y] = false;
            rowsSolved[nextCoord.y] = false;
        }
        testLabel.text = "art is scrambled";
    }

    void swapBalls(Vector2Int coord1, Vector2Int coord2, bool swapPositions = true) {
        var first = balls[coord1.x, coord1.y];
        var second = balls[coord2.x, coord2.y];
        balls[coord1.x, coord1.y] = second;
        balls[coord2.x, coord2.y] = first;
        if (!swapPositions) return;
        first.setPosition(view.getBallPosition(coord2), coord2);
        second.setPosition(view.getBallPosition(coord1), coord1);
    }

    Vector2Int randomCoordinate(Vector2Int initialCoord) {
        var coord = new Vector2Int();
        do {
            coord.x = RandomUtils.nextInt(0, artSize.x);
            coord.y = RandomUtils.nextInt(0, artSize.y);
        } while (coord == initialCoord);
        return coord;
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

    void resetBalls() {
        for (var x = 0; x < artSize.x; x++) {
            for (var y = 0; y < artSize.y; y++) {
                var ball = balls[x, y];
                ball.setPosition(view.getBallPosition(ball.initialPosition), ball.initialPosition);
                balls[x, y] = ballsCopy[x, y];
            }
        }
        for (var i = 0; i < rowsSolved.Length; i++) rowsSolved[i] = true;
        testLabel.text = "art is reset";
    }
    #endregion

    #region input handling
    public void onVerticalSwipe(bool up, int rowIndex, Vector3 position) {
        var nextRowIndex = up ? rowIndex + 1 : rowIndex - 1;
        if (nextRowIndex < 0 || nextRowIndex >= artSize.y) return;
        var coord = getClosestBallPositionTo(position, rowIndex);
        var nextCoord = new Vector2Int(coord.x, nextRowIndex);
        animateVerticalSwipe(coord, nextCoord, () => checkIfComplete(coord.y, nextCoord.y));
        soundService.playSound(SoundId.BallSwipeClick);
        vibrationService.vibrate(VibrationType.Light);
    }

    Vector2Int getClosestBallPositionTo(Vector3 position, int rowIndex) {
        var minDistance = float.MaxValue;
        var ballIndex = 0;
        for (var x = 0; x < artSize.x; x++) {
            var ball = balls[x, rowIndex];
            var distance = position.distanceTo(ball.position);
            if (distance < minDistance) {
                minDistance = distance;
                ballIndex = x;
            }
        }
        return new Vector2Int(ballIndex, rowIndex);
    }

    public void onHorizontalShift(int rowIndex, float deltaX) {
        var maxDeltaX = view.getBallSize().x / 2 + settings.distanceBetweenBalls / 2;
        if (Mathf.Abs(deltaX) > maxDeltaX) {
            deltaX = maxDeltaX * Mathf.Sign(deltaX);
        }
        for (var x = 0; x < artSize.x; x++) {
            var ball = balls[x, rowIndex];
            ball.transform.Translate(deltaX, 0, 0);
        }
        var first = balls[0, rowIndex];
        var last = balls[artSize.x - 1, rowIndex];
        updateLeftEdgeBall(first, last);
        updateRightEdgeBall(first, last);
        if (deltaX < 0) { // left shift
            if (first.position.x < view.widthRange.min) {
                first.transform.position = new Vector3(last.position.x + view.step, last.position.y);
                shiftBallsInArray(rowIndex, true);
                updateLeftEdgeBall(balls[0, rowIndex], first); // first becomes last after the shift
                soundService.playSound(SoundId.BallSwipeClick);
                vibrationService.vibrate(VibrationType.Light);
            }
        } else {
            if (last.position.x > view.widthRange.max) {
                last.transform.position = new Vector3(first.position.x - view.step, first.position.y);
                shiftBallsInArray(rowIndex, false);
                updateRightEdgeBall(last, balls[artSize.x - 1, rowIndex]); // last becomes first after the shift
                soundService.playSound(SoundId.BallSwipeClick);
                vibrationService.vibrate(VibrationType.Light);
            }
        }
    }

    void updateLeftEdgeBall(Ball first, Ball last) {
        leftEdgeBall.transform.position = new Vector3(first.position.x - view.step, first.position.y);
        leftEdgeBall.color = last.color;
    }
    
    void updateRightEdgeBall(Ball first, Ball last) {
        rightEdgeBall.transform.position = new Vector3(last.position.x + view.step, last.position.y);
        rightEdgeBall.color = first.color;
    }
    
    public void onSwipeEnd(Direction swipeDirection, int rowIndex) {
        if (swipeDirection.isHorizontal()) {
            var duration = -1f;
            for (var x = 0; x < artSize.x; x++) {
                var ball = balls[x, rowIndex];
                var position = view.getBallPosition(x, rowIndex);
                if (duration < 0f) {
                    duration = Mathf.Abs(ball.position.x - position.x) / animationSettings.horizontalReturnSpeed;
                }
                StartCoroutine(Coroutines.moveTo(ball.transform, position, duration));
            }
            var firstBallPosition = view.getBallPosition(0, rowIndex);
            var leftEndPosition = new Vector3(firstBallPosition.x - view.step, firstBallPosition.y);
            StartCoroutine(Coroutines.moveTo(leftEdgeBall.transform, leftEndPosition, duration));
            var lastBallPosition = view.getBallPosition(artSize.x - 1, rowIndex);
            var rightEndPosition = new Vector3(lastBallPosition.x + view.step, lastBallPosition.y);
            StartCoroutine(Coroutines.moveTo(rightEdgeBall.transform, rightEndPosition, duration, () => {
                checkIfComplete(rowIndex);
            }));
        }
    }

    void animateVerticalSwipe(Vector2Int coord1, Vector2Int coord2, Action action) {
        var first = balls[coord1.x, coord1.y];
        var second = balls[coord2.x, coord2.y];
        var position1 = view.getBallPosition(coord1);
        var position2 = view.getBallPosition(coord2);
        var duration = animationSettings.verticalSwipeDuration;
        first.transform.setZ(-1);
        StartCoroutine(Coroutines.moveTo(first.transform, position2, duration, () => {
            first.transform.setZ(0);
            swapBalls(coord1, coord2, false);
            action.Invoke();
        }));
        StartCoroutine(Coroutines.moveTo(second.transform, position1, duration));
    }
    #endregion

    #region internals
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

    void checkIfComplete(int rowIndex, int secondRowIndex = -1) {
        checkRow(rowIndex);
        if (secondRowIndex != -1) checkRow(secondRowIndex);
        artSolved = rowsSolved.All(rowSolved => rowSolved);
        testLabel.text = artSolved ? "art is solved!" : "solving..";
        if (artSolved) log.log("art is solved");
    }

    void checkRow(int rowIndex) {
        var y = rowIndex;
        var complete = true;
        for (var x = 0; x < artSize.x; x++) {
            if (balls[x, y].color.approximately(artView.getPixelColor(x, y))) continue;
            complete = false;
            break;
        }
        rowsSolved[rowIndex] = complete;
    }
    #endregion
}
}