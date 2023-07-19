using UnityEngine;
using Utils;

namespace game_scene.controllers {
public class BallScript : MonoBehaviour {
    [SerializeField] float duration;

    Vector3 end = new(1.5f, 2);

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var position = transform.position;
            // var middle = transform.position.midPoint(end);
            // StartCoroutine(Coroutines.moveTo(transform, middle, duration / 2, () => {
            // StartCoroutine(Coroutines.moveTo(transform, end, duration / 2, null, Interpolation.Decelerate));
            // }, Interpolation.Accelerate));
                StartCoroutine(Coroutines.moveTo(transform, end, duration, Interpolation.AccelerateDecelerate, () => {
                end = position;
            }));
        }
    }
}
}