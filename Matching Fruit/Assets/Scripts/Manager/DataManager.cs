using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private GameObject m_Boss;
    [SerializeField] private IngameObject m_EmptyObject;
    [SerializeField] private List<Level> m_Levels = new List<Level>();

    private int m_CurrLevelIndex;
    private int m_Score = 0;
    private int m_Star = 0;
    private int m_CurrGoal = 0;
    private int m_MissionCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        SetCurrentLevel(PlayerPrefs.GetInt(Define.CURRENT_LEVEL_KEY, Define.LEVEL_DEFAULT));
        if (m_Levels[m_CurrLevelIndex].isBossLevel == true)
            m_Boss.SetActive(true);
    }

    private void SetCurrentLevel(int level)
    {
        m_CurrLevelIndex = level;
    }

    public int GetNextLevel()
    {
        if (m_CurrLevelIndex + 1 >= m_Levels.Count)
            return -1;
        return m_CurrLevelIndex + 1;
    }

    public void SetMatrixData(ref int row, ref int col, ref GameObject prefab)
    {
        row = m_Levels[m_CurrLevelIndex].row;
        col = m_Levels[m_CurrLevelIndex].col;
        prefab = m_Levels[m_CurrLevelIndex].prefab;
    }

    public void SetMatrixState(ref Matrix.MatrixState[,] matrixStates, ref Object[,] matrix)
    {
        for (int i = 0; i < m_Levels[m_CurrLevelIndex].row; i++)
        {
            for (int j = 0; j < m_Levels[m_CurrLevelIndex].col; j++)
            {
                Color currCell = m_Levels[m_CurrLevelIndex].map.GetPixel(j, i);
                if (currCell == Color.white)
                {
                    matrixStates[i, j] = Matrix.MatrixState.None;
                }
                if (currCell == Color.red)
                {
                    matrixStates[i, j] = Matrix.MatrixState.Block;
                    matrix[i, j].SetStateBlock();
                }
                if (currCell == Color.blue)
                {
                    matrixStates[i, j] = Matrix.MatrixState.Chained;
                    matrix[i, j].SetStateChained();
                }
                if (currCell == Color.green)
                {
                    matrixStates[i, j] = Matrix.MatrixState.Freeze;
                    matrix[i, j].SetStateFreeze();
                }
            }
        }
    }

    public void SetLevelData(ref int score, ref float time, ref int turns, ref int balance)
    {
        score = 0;
        time = m_Levels[m_CurrLevelIndex].limitedTime;
        turns = m_Levels[m_CurrLevelIndex].turns;
        balance = m_Levels[m_CurrLevelIndex].startBalance;

        PlayScene.instance.m_Score.SetText(score.ToString());
        PlayScene.instance.m_Time.SetText(Mathf.Floor(time).ToString());
        PlayScene.instance.m_Turns.SetText(turns.ToString());
        PlayScene.instance.m_Balance.SetText(balance.ToString());
    }

    public int CheckGoal(int score)
    {
        m_Score = score;
        if (score >= m_Levels[m_CurrLevelIndex].goal_1 && m_CurrGoal < 1)
        {
            m_Star = 1;
            m_CurrGoal++;
            PlayScene.instance.m_FirstStar.SetUnlock(true);
            PlayScene.instance.m_ResultFirstStar.SetUnlock(true);
        }
        if (score >= m_Levels[m_CurrLevelIndex].goal_2 && m_CurrGoal < 2)
        {
            m_Star = 2;
            m_CurrGoal++;
            PlayScene.instance.m_SecondStar.SetUnlock(true);
            PlayScene.instance.m_ResultSecondStar.SetUnlock(true);
        }
        if (score >= m_Levels[m_CurrLevelIndex].goal_3 && m_CurrGoal < 3)
        {
            m_Star = 3;
            m_CurrGoal++;
            PlayScene.instance.m_ThirdStar.SetUnlock(true);
            PlayScene.instance.m_ResultThirdStar.SetUnlock(true);
        }

        return m_CurrGoal;
    }

    public IngameObject GetEmptyObject()
    {
        return m_EmptyObject;
    }

    public IngameObject GetRandomObject()
    {
        bool isRare = ((float)Random.Range(0, 100) / 100f) < m_Levels[m_CurrLevelIndex].rareObjectsPercentage ? true : false;
        if (isRare)
            return GetRandomRareObject();
        else
            return GetRandomCommonObject();
    }

    public IngameObject GetRandomCommonObject()
    {
        return m_Levels[m_CurrLevelIndex].commmonObjects[Random.Range(0, m_Levels[m_CurrLevelIndex].commmonObjects.Count)];
    }

    public IngameObject GetRandomRareObject()
    {
        float rand = ((float)Random.Range(0, 100) / 100f);
        float rate = 0f;
        foreach (IngameObjectRate ingameObbjectRate in m_Levels[m_CurrLevelIndex].rareObjects)
        {
            rate += ingameObbjectRate.percentage;
            if (rand < rate)
                return ingameObbjectRate.ingameObject;
        }
        return m_Levels[m_CurrLevelIndex].rareObjects[0].ingameObject;
    }

    public ClearMission GetRandomClearMission()
    {
        m_MissionCount++;
        ClearMission rand = Instantiate(m_Levels[m_CurrLevelIndex].clearMissions[Random.Range(0, m_Levels[m_CurrLevelIndex].clearMissions.Count)]);
        rand.counter *= (int)Mathf.Ceil(m_MissionCount / 3);
        rand.price *= (int)Mathf.Ceil(m_MissionCount / 3);
        return rand;
    }

    public void SaveGame()
    {
        string jsonString;
        if (File.Exists(Define.SAVE_FILE))
        {
            jsonString = File.ReadAllText(Define.SAVE_FILE);
        }
        else
        {
            Debug.Log("file does not exist !");
            jsonString = "";
        }
        LevelProgress[] progresses = JsonHelper.FromJson<LevelProgress>(jsonString);

        progresses[m_CurrLevelIndex].score = progresses[m_CurrLevelIndex].score < m_Score ? m_Score : progresses[m_CurrLevelIndex].score;
        progresses[m_CurrLevelIndex].star = progresses[m_CurrLevelIndex].star < m_Star ? m_Star : progresses[m_CurrLevelIndex].star;
        if (m_CurrLevelIndex + 1 < progresses.Length && m_Star > 0)
            progresses[m_CurrLevelIndex + 1].isUnlock = true;

        jsonString = JsonHelper.ToJson(progresses, true);
        File.WriteAllText(Define.SAVE_FILE, jsonString);
    }
}
