using UnityEngine;

namespace game_scene.models {
public class Ball : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    public Vector2Int gridPosition { get; private set; }

    public Color color {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void setPosition(Vector3 position, int gridX, int gridY) {
        transform.position = position;
        gridPosition = new Vector2Int(gridX, gridY);
    }
}
}