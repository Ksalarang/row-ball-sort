using game_scene.controllers;
using UnityEngine;
using Utils;
using Zenject;

namespace game_scene {
public class GameController : MonoBehaviour {
    [Inject] BallFactory ballFactory;
    
    Log log;

    void Awake() {
        log = new(GetType());
        Application.targetFrameRate = 60;
    }

    void Start() {
        log.log("start");
    }
}
}