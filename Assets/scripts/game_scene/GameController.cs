using UnityEngine;
using Utils;

namespace game_scene {
public class GameController : MonoBehaviour {
    Log log;

    void Awake() {
        log = new(GetType());
    }

    void Start() {
        log.log("start");
    }
}
}