using game_scene.models;
using UnityEngine;
using Zenject;

namespace game_scene.controllers {
public class BallFactory : MonoBehaviour {
    [Inject(Id = PrefabId.Ball)] GameObject ballPrefab;
    [Inject] DiContainer container;

    public Ball create(string name = "ball") {
        var ball = container.InstantiatePrefabForComponent<Ball>(ballPrefab);
        ball.name = name;
        return ball;
    }
}
}