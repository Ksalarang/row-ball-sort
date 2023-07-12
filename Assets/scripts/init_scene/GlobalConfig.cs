using System;

namespace init_scene {
[Serializable]
public class GlobalConfig {
    public LogConfig logConfig;
}

[Serializable]
public struct LogConfig {
    public bool sceneService;
    public bool soundService;
    public bool vibrationService;
}
}