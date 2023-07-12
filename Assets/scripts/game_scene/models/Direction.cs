using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace game_scene.models {
public enum Direction {
    Up, Down, Left, Right,
}

public static class DirectionExtensions {
    public static Vector2Int toVector(this Direction direction) {
        var vector = new Vector2Int();
        switch (direction) {
            case Direction.Up:
                vector.y = 1;
                break;
            case Direction.Down:
                vector.y = -1;
                break;
            case Direction.Left:
                vector.x = -1;
                break;
            case Direction.Right:
                vector.x = 1;
                break;
        }
        return vector;
    }

    public static bool isVertical(this Direction direction) => direction is Direction.Up or Direction.Down;

    public static bool isHorizontal(this Direction direction) => direction is Direction.Left or Direction.Right;
}
}