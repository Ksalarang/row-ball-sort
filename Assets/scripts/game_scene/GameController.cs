using services.sounds;
using UnityEngine;
using Utils;
using Zenject;

namespace game_scene {
public class GameController : MonoBehaviour {
    [Inject] SoundService soundService;
    
    Log log;

    void Awake() {
        log = new(GetType());
        Application.targetFrameRate = 60;
    }

    void Start() {
        log.log("start");
        soundService.playSoundtrack(SoundTrackId.Background);
    }
}
}