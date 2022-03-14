using UnityEngine;

public static class Define
{
    public static string CURRENT_LEVEL_KEY = "current_level";
    public static int LEVEL_DEFAULT = 0;

    public static string SCENE_SELECT_LEVEL = "SelectLevelScene";
    public static string SCENE_PLAY = "PlayScene";

    public static string SCENE_FADER_KEY = "isStartFading";

    public static string SAVE_FILE = Application.persistentDataPath + "/save_file.json";
    public static string CONFIG_FILE = Application.persistentDataPath + "/config_file.json";
}
