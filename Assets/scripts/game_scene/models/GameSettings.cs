using System;
using UnityEngine;

namespace game_scene.models {
public class GameSettings : MonoBehaviour {
    public ArtScrambleSettings artScrambleSettings;
    public InputSettings inputSettings;
    public BallAnimationSettings ballAnimationSettings;
    public BallAreaSettings ballAreaSettings;
}

[Serializable]
public struct ArtScrambleSettings {
    [Range(0f, 1f)] public float scramblePercentage;
}

[Serializable]
public struct InputSettings {
    [Range(0.1f, 1f)] public float verticalSwipeSensitivity;
    [Range(0.1f, 1f)] public float horizontalSwipeSensitivity;
}

[Serializable]
public struct BallAnimationSettings {
    public float verticalSwipeDuration;
}

[Serializable]
public struct BallAreaSettings {
    public float distanceBetweenBalls;
}
}