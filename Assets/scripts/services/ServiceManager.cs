using System.Collections.Generic;
using services.saves;
using services.scenes;
using services.sounds;
using services.vibrations;
using UnityEngine;
using utils.interfaces;
using Zenject;

namespace services {
public class ServiceManager: MonoBehaviour {
    [Inject] SceneService sceneService;
    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;
    [Inject] PlayerPrefsService playerPrefsService;

    readonly List<AppLifecycleListener> appLifecycleListeners = new();
    readonly List<PlayerPrefsLoadListener> prefsLoadListeners = new();

    void Awake() {
        registerServices();
    }

    void registerServices() {
        registerService(sceneService);
        registerService(soundService);
        registerService(vibrationService);
        registerService(playerPrefsService);
        onPrefsLoaded();
    }

    void registerService(Service service) {
        if (service is AppLifecycleListener appLifecycleListener) {
            appLifecycleListeners.Add(appLifecycleListener);
        }
        if (service is PlayerPrefsLoadListener prefsLoadListener) {
            prefsLoadListeners.Add(prefsLoadListener);
        }
    }

    void onPrefsLoaded() {
        var prefs = playerPrefsService.getPrefs();
        foreach (var listener in prefsLoadListeners) {
            listener.onPrefsLoaded(prefs);
        }
    }

    void OnApplicationQuit() {
        foreach (var listener in appLifecycleListeners) {
            listener.onQuit();
        }
    }
}

public interface Service {}
}