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
    readonly Dictionary<SoundTrackId, SoundTrack> soundtracks = new();
    readonly GameObject soundContainer = new("sounds");
    readonly GameObject soundtrackContainer = new("tracks");

    [Inject]
    public SoundService(AudioSources sources, LogConfig logConfig) {
        log = new(GetType(), logConfig.soundService);
        this.sources = sources;
        Object.DontDestroyOnLoad(soundContainer);
        Object.DontDestroyOnLoad(soundtrackContainer);
        initSounds();
        initSoundTracks();
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

    void initSoundTracks() {
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
        sound.play();
        log.log($"play {sound}");
    }

    public void playSoundtrack(SoundTrackId id) {
        var soundtrack = soundtracks[id];
        soundtrack.play();
        log.log($"play {soundtrack}");
    }

    public void stopSoundtrack(SoundTrackId id) {
        var soundtrack = soundtracks[id];
        soundtrack.stop();
        log.log($"stop {soundtrack}");
    }
}

[Serializable]
public class AudioSources {
    public Sound[] sounds;
    public SoundTrack[] soundtracks;
}

[Serializable]
public class Sound {
    public SoundId id;
    public AudioClip clip;
    [HideInInspector] public AudioSource audioSource;

    public void play() {
        audioSource.PlayOneShot(audioSource.clip);
    }

    public override string ToString() => $"sound {id}";
}

[Serializable]
public class SoundTrack {
    public SoundTrackId id;
    public AudioClip clip;
    [HideInInspector] public AudioSource audioSource;

    public void play(bool loop = true) {
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void stop() {
        audioSource.Stop();
    }

    public override string ToString() => $"soundtrack {id}";
}

public enum SoundId {
    BallSwipeClick,
}

public enum SoundTrackId {
    Background,
}
}