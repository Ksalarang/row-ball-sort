using System;
using init_scene;
using services.saves;
using UnityEngine;
using Utils;
using Zenject;

namespace services.vibrations {
public class VibrationService : Service, PlayerPrefsLoadListener {

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
    PlayerPrefsData prefs;

    [Inject]
    public VibrationService(LogConfig logConfig) {
        log = new(GetType(), logConfig.vibrationService);
        log.log("create");
    }

    public void onPrefsLoaded(PlayerPrefsData prefs) {
        this.prefs = prefs;
    }

    public void vibrate(VibrationType type) {
        if (!prefs.vibrationEnabled) return;
        log.log($"vibrate: {type}");
        vibrate(getVibrationDuration(type));
    }
    
    public void setVibrationEnabled(bool enabled) {
        prefs.vibrationEnabled = enabled;
    }

    public bool isVibrationEnabled() => prefs.vibrationEnabled;

    public bool supportsVibration() => SystemInfo.supportsVibration;

    void vibrate(long milliseconds) {
        if (isAndroid) vibrator.Call("vibrate", milliseconds);
        // else Handheld.Vibrate();
    }

    void vibrate(long[] pattern, int repeat) {
        if (isAndroid) vibrator.Call("vibrate", pattern, repeat);
        // else Handheld.Vibrate();
    }

    void cancel() {
        if (isAndroid) vibrator.Call("cancel");
    }

    long getVibrationDuration(VibrationType type) {
        return type switch {
            VibrationType.Light => 20,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    void doNotCallThisMethod() {
        Handheld.Vibrate(); // for vibration permission
    }
}

public enum VibrationType {
    Light,
}
}