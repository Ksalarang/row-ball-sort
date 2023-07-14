using System.Collections.Generic;
using init_scene;
using services.saves;
using services.scenes;
using services.sounds;
using services.vibrations;
using UnityEngine;
using Utils;
using utils.interfaces;
using Zenject;

namespace services {
public class ServiceManager: MonoBehaviour {
    [Inject] LogConfig logConfig;
    
    [Inject] SceneService sceneService;
    [Inject] SoundService soundService;
    [Inject] VibrationService vibrationService;
    [Inject] PlayerPrefsService playerPrefsService;

    Log log;
    List<AppLifecycleListener> appLifecycleListeners;
    List<PlayerPrefsLoadListener> prefsLoadListeners;

    void Awake() {
        log = new(GetType(), logConfig.serviceManager);
        appLifecycleListeners = new();
        prefsLoadListeners = new();
        registerServices();
    }

    void registerServices() {
        log.log("register services");
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
        log.log("on prefs loaded");
        var prefs = playerPrefsService.getPrefs();
        foreach (var listener in prefsLoadListeners) {
            listener.onPrefsLoaded(prefs);
        }
    }

    void OnApplicationPause(bool pauseStatus) {
        log.log("on pause");
        foreach (var listener in appLifecycleListeners) {
            listener.onPause();
        }
    }

    void OnApplicationQuit() {
        log.log("on quit");
        foreach (var listener in appLifecycleListeners) {
            listener.onQuit();
        }
    }
}

public interface Service {}
}