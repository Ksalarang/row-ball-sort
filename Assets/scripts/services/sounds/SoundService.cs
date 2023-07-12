using System;
using System.Collections.Generic;
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
    readonly GameObject soundContainer = new("sounds");

    [Inject]
    public SoundService(AudioSources sources) {
        log = new(GetType());
        this.sources = sources;
        Object.DontDestroyOnLoad(soundContainer);
        initSounds();
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

    public void playSound(SoundId id) {
        var sound = sounds[id];
        sound.play();
        log.log($"play {sound}");
    }
}

[Serializable]
public class AudioSources {
    public Sound[] sounds;
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

public enum SoundId {
    BallSwipeClick,
}
}