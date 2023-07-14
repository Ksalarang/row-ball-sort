using System;
using System.Collections.Generic;
using init_scene;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;
using Object = UnityEngine.Object;

namespace services.sounds {
public class SoundService {
    readonly Log log;
    readonly AudioSources sources;
    readonly Dictionary<SoundId, Sound> sounds = new();
    readonly Dictionary<SoundTrackId, Soundtrack> soundtracks = new();
    readonly GameObject soundContainer = new("sounds");
    readonly GameObject soundtrackContainer = new("soundtracks");
    
    float soundVolume = 1;
    float musicVolume = 1;
    Soundtrack currentSoundtrack;

    [Inject]
    public SoundService(AudioSources sources, LogConfig logConfig) {
        log = new(GetType(), logConfig.soundService);
        this.sources = sources;
        Object.DontDestroyOnLoad(soundContainer);
        Object.DontDestroyOnLoad(soundtrackContainer);
        initSounds();
        initSoundtracks();
    }

    void initSounds() {
        foreach (var sound in sources.sounds) {
            var soundObject = new GameObject(sound.id.ToString());
            Object.DontDestroyOnLoad(soundObject);
            soundObject.transform.SetParent(soundContainer.transform);
            sound.audioSource = soundObject.AddComponent<AudioSource>();
            sound.audioSource.playOnAwake = false;
            sound.audioSource.clip = sound.clip;
            sounds.Add(sound.id, sound);
        }
        log.log($"init sounds: {sounds.Values.toString()}");
    }

    void initSoundtracks() {
        foreach (var soundtrack in sources.soundtracks) {
            var soundtrackObject = new GameObject(soundtrack.id.ToString());
            Object.DontDestroyOnLoad(soundtrackObject);
            soundtrackObject.transform.SetParent(soundtrackContainer.transform);
            soundtrack.audioSource = soundtrackObject.AddComponent<AudioSource>();
            soundtrack.audioSource.playOnAwake = false;
            soundtrack.audioSource.clip = soundtrack.clip;
            soundtracks.Add(soundtrack.id, soundtrack);
        }
        log.log($"init soundtracks: {soundtracks.Values.toString()}");
    }

    public void playSound(SoundId id) {
        var sound = sounds[id];
        sound.play(soundVolume);
        log.log($"play {sound}");
    }

    public void playSoundtrack(SoundTrackId id) {
        var soundtrack = soundtracks[id];
        soundtrack.play(musicVolume, true);
        currentSoundtrack = soundtrack;
        log.log($"play {soundtrack}");
    }

    public void stopSoundtrack(SoundTrackId id) {
        var soundtrack = soundtracks[id];
        soundtrack.stop();
        log.log($"stop {soundtrack}");
    }

    public void setSoundVolume(float value) => soundVolume = value;

    public float getSoundVolume() => soundVolume;

    public void setMusicVolume(float value) {
        musicVolume = value;
        if (currentSoundtrack != null) currentSoundtrack.volume = value;
    }

    public float getMusicVolume() => musicVolume;
}

[Serializable]
public class AudioSources {
    public Sound[] sounds;
    public Soundtrack[] soundtracks;
}

[Serializable]
public class Sound : Audio {
    public SoundId id;

    public void play(float volume) {
        playOneShot(volume);
    }

    public override string ToString() => $"sound {id}";
}

[Serializable]
public class Soundtrack : Audio {
    public SoundTrackId id;

    public override string ToString() => $"soundtrack {id}";
}

public abstract class Audio {
    public AudioClip clip;
    [HideInInspector] public AudioSource audioSource;

    public float volume {
        get => audioSource.volume;
        set => audioSource.volume = value;
    }

    public void play(float volume, bool loop) {
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
    }
    
    protected void playOneShot(float volume) {
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
    }
    
    public void stop() {
        audioSource.Stop();
    }
}

public enum SoundId {
    BallSwipeClick,
}

public enum SoundTrackId {
    Background,
}
}