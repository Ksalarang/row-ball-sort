using System;
using UnityEngine;

namespace game_scene.models {
public class GameSettings : MonoBehaviour {
    public ArtScrambleSettings artScrambleSettings;
}

[Serializable]
public struct ArtScrambleSettings {
    [Range(0f, 1f)] public float scramblePercentage;
}
}