using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Config
{
    public bool isMute;
}

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager instance;
    public Config config;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        string jsonString;
        if (File.Exists(Define.CONFIG_FILE))
        {
            jsonString = File.ReadAllText(Define.CONFIG_FILE);
            config = JsonUtility.FromJson<Config>(jsonString);
        }
        else
        {
            Debug.Log("config file does not exist !");
            LoadDefaultConfigData();
            jsonString = JsonUtility.ToJson(config, true);
            File.WriteAllText(Define.CONFIG_FILE, jsonString);
        }
    }

    public void ResetConfig()
    {
        LoadDefaultConfigData();
        string jsonString = JsonUtility.ToJson(config, true);
        File.WriteAllText(Define.CONFIG_FILE, jsonString);
    }

    public void SaveConfig()
    {
        string jsonString = JsonUtility.ToJson(config, true);
        File.WriteAllText(Define.CONFIG_FILE, jsonString);
    }

    private void LoadDefaultConfigData()
    {
        config = new Config();
        config.isMute = false;
    }
}
