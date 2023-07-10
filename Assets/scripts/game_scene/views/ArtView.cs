using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace game_scene.views {
public class ArtView : MonoBehaviour {
    [SerializeField] Image artImage;

    [Inject] BallAreaView ballAreaView;

    Log log;
    Sprite artSprite;
    Vector2Int artSize;

    void Awake() {
        log = new(GetType());
        artSprite = artImage.sprite;
        artSize = new Vector2Int(artSprite.texture.width, artSprite.texture.width);
        ballAreaView.onArtDataReady();
    }

    public Vector2Int getArtSizeInPixels() => artSize;

    public Color getPixelColor(int x, int y) => artSprite.texture.GetPixel(x, y);
}
}