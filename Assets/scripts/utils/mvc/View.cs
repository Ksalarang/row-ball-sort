using System;
using UnityEngine;
using Utils.Interfaces;

namespace Utils.MVC {
public abstract class View : MonoBehaviour, AlphaAdjustable {
    protected const float FadeDuration = 0.2f;
    [SerializeField] protected CanvasGroup canvasGroup;

    public float alpha {
        get => canvasGroup.alpha;
        set => canvasGroup.alpha = value;
    }

    public void show() {
        gameObject.SetActive(true);
    }

    public void hide() {
        gameObject.SetActive(false);
    }

    protected virtual void show(float duration, Action action = null) {
        show();
        canvasGroup.interactable = false;
        alpha = 0f;
        StartCoroutine(Coroutines.fadeTo(this, 1f, duration, () => {
            canvasGroup.interactable = true;
            action?.Invoke();
        }));
    }

    protected virtual void hide(float duration, Action action = null) {
        canvasGroup.interactable = false;
        alpha = 1f;
        StartCoroutine(Coroutines.fadeTo(this, 0f, duration, () => {
            canvasGroup.interactable = true;
            hide();
            action?.Invoke();
        }));
    }
}
}