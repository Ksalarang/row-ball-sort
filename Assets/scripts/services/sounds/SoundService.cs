using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.Extensions;
using Zenject;

namespace services.sounds {
public class SoundService {
    readonly Log log;
    readonly AudioSources sources;
    readonly Dictionary<SoundId, Sound> sounds = new();
    readonly GameObject soundContainer = new();

    [Inject]
    public SoundService(AudioSources sources) {
        log = new(GetType());
        this.sources = sources;
        initSounds();
    }

    void initSounds() {
        foreach (var sound in sources.sounds) {
            var soundObject = new GameObject();
            soundObject.transform.SetParent(soundContainer.transform);
            sound.audioSource = soundObject.AddComponent<AudioSource>();
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