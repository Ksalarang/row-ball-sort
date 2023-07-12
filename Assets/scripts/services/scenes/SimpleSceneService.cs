using UnityEngine.SceneManagement;

namespace services.scenes {
public class SimpleSceneService: SceneService {

    public void loadGameScene() {
        SceneManager.LoadScene("GameScene");
    }
}
}