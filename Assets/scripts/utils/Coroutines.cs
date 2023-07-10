using System;
using System.Collections;
using UnityEngine;
using Utils.Interfaces;

namespace Utils {
public static class Coroutines {
    public static IEnumerator moveTo(Transform transform, Vector3 end, float duration, Action action = null) {
        var start = transform.position;
        var time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time / duration);
            yield return null;
        }
        action?.Invoke();
    }

    public static IEnumerator scaleTo(Transform transform, Vector3 end, float duration, Action action = null) {
        var start = transform.localScale;
        var time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, time / duration);
            yield return null;
        }
        action?.Invoke();
    }

    public static IEnumerator fadeTo(Colorable colorable, float alpha, float duration, Action action = null) {
        var start = colorable.color;
        var end = new Color(start.r, start.g, start.b, alpha);
        var time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            colorable.color = Color.Lerp(start, end, time / duration);
            yield return null;
        }
        action?.Invoke();
    }

    public static IEnumerator fadeTo(AlphaAdjustable alphaAdjustable, float alpha, float duration, Action action = null) {
        var start = alphaAdjustable.alpha;
        var time = 0f;
        while (time < duration) {
            time += Time.deltaTime;
            alphaAdjustable.alpha = Mathf.Lerp(start, alpha, time / duration);
            yield return null;
        }
        action?.Invoke();
    }

    public static IEnumerator delayAction(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
}
}