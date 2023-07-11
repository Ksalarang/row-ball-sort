using UnityEngine;

namespace game_scene.models {
public class Ball : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    [HideInInspector] public Vector2Int initialPosition;
    
    public Vector2Int gridPosition { get; private set; }
    public Vector3 position => transform.position;

    public Color color {
        get => spriteRenderer.color;
        set => spriteRenderer.color = value;
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = new Vector2Int(-1, -1);
    }

    public void setPosition(Vector3 position, int gridX, int gridY) {
        setPosition(position, new Vector2Int(gridX, gridY));
    }

    public void setPosition(Vector3 position, Vector2Int gridPosition) {
        transform.position = position;
        this.gridPosition = gridPosition;
    }
}
}