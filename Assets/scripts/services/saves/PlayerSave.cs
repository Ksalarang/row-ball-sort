using System;
using UnityEngine;

namespace services.saves {
[Serializable]
public class PlayerSave {
    public AudioSave audio;

    public string toJson() => JsonUtility.ToJson(this);

    public void fromJson(string json) => JsonUtility.FromJsonOverwrite(json, this);
}

[Serializable]
public class AudioSave {
    public float soundVolume;
    public float musicVolume;
    public bool vibrationEnabled;
}
}