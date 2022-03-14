using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    public static RecordManager instance;

    [SerializeField] private int levelCount = 9;
    private LevelProgress[] progresses;
    public LevelProgress[] Progresses => progresses;

    void Awake()
    {
        if (instance == null)
            instance = this;

        string jsonString;
        if (File.Exists(Define.SAVE_FILE))
        {
            jsonString = File.ReadAllText(Define.SAVE_FILE);
            progresses = JsonHelper.FromJson<LevelProgress>(jsonString);
        }
        else
        {
            Debug.Log("save file does not exist !");
            LoadDefaultSaveData();
            jsonString = JsonHelper.ToJson(progresses, true);
            File.WriteAllText(Define.SAVE_FILE, jsonString);
        }
    }

    public void ResetData()
    {
        LoadDefaultSaveData();
        string jsonString = JsonHelper.ToJson(progresses, true);
        File.WriteAllText(Define.SAVE_FILE, jsonString);
    }

    public void CheatLevel()
    {
        for (int i = 0; i < levelCount; i++)
        {
            progresses[i].isUnlock = true;
        }
        string jsonString = JsonHelper.ToJson(progresses, true);
        File.WriteAllText(Define.SAVE_FILE, jsonString);
    }

    public void SaveData()
    {
        string jsonString = JsonHelper.ToJson(progresses, true);
        File.WriteAllText(Define.SAVE_FILE, jsonString);
    }

    private void LoadDefaultSaveData()
    {
        progresses = new LevelProgress[levelCount];
        for (int i = 0; i < levelCount; i++)
        {
            progresses[i] = new LevelProgress();
            progresses[i].isUnlock = false;
            progresses[i].score = 0;
            progresses[i].star = 0;
        }
        progresses[0].isUnlock = true;
    }
}
