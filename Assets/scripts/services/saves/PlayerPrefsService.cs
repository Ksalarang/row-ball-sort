using UnityEngine;
using Utils.Extensions;
using utils.interfaces;

namespace services.saves {
public class PlayerPrefsService : Service, AppLifecycleListener {
    readonly PlayerPrefsData prefs;

    public PlayerPrefsService() {
        prefs = new PlayerPrefsData {
            audio = new AudioPrefs {
                soundVolume = PlayerPrefs.GetFloat(Keys.SoundVolume, 1f),
                musicVolume = PlayerPrefs.GetFloat(Keys.MusicVolume, 1f),
                vibrationEnabled = PlayerPrefsExt.getBool(Keys.VibrationEnabled, true),
            },
        };
    }

    public PlayerPrefsData getPrefs() => prefs;

    public void onQuit() {
        PlayerPrefs.SetFloat(Keys.SoundVolume, prefs.audio.soundVolume);
        PlayerPrefs.SetFloat(Keys.MusicVolume, prefs.audio.musicVolume);
        PlayerPrefsExt.setBool(Keys.VibrationEnabled, prefs.audio.vibrationEnabled);
        PlayerPrefs.Save();
    }

    static class Keys {
        public const string SoundVolume = "soundVolume";
        public const string MusicVolume = "musicVolume";
        public const string VibrationEnabled = "vibrationEnabled";
    }
}

public class PlayerPrefsData {
    public AudioPrefs audio;
}

public class AudioPrefs {
    public float soundVolume;
    public float musicVolume;
    public bool vibrationEnabled;
}
}