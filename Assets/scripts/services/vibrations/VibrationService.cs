using System;
using init_scene;
using UnityEngine;
using Utils;
using Zenject;

namespace services.vibrations {
public class VibrationService {

#if UNITY_ANDROID && !UNITY_EDITOR
    static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
    static readonly bool isAndroid = true;
#else
    static AndroidJavaClass unityPlayer;
    static AndroidJavaObject currentActivity;
    static AndroidJavaObject vibrator;
    static readonly bool isAndroid = false;
#endif
    
    readonly Log log;

    [Inject]
    public VibrationService(LogConfig logConfig) {
        log = new(GetType(), logConfig.vibrationService);
        log.log("create");
    }

    public void vibrate(VibrationType type) {
        log.log($"vibrate: {type}");
        vibrate(getVibrationDuration(type));
    }

    void vibrate(long milliseconds) {
        if (isAndroid) vibrator.Call("vibrate", milliseconds);
        else Handheld.Vibrate();
    }

    void vibrate(long[] pattern, int repeat) {
        if (isAndroid) vibrator.Call("vibrate", pattern, repeat);
        else Handheld.Vibrate();
    }

    void cancel() {
        if (isAndroid) vibrator.Call("cancel");
    }

    long getVibrationDuration(VibrationType type) {
        return type switch {
            VibrationType.Light => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public enum VibrationType {
    Light,
}
}