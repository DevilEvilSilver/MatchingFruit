using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private List<Level> m_Levels = new List<Level>();

    private Level m_CurrLevel;
    private int m_MissionCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        SetCurrentLevel(PlayerPrefs.GetInt(Define.CURRENT_LEVEL_KEY, Define.LEVEL_1));
    }

    private void SetCurrentLevel(int level)
    {
        m_CurrLevel = m_Levels[level];
    }

    public IngameObject GetRandomObject()
    {
        bool isRare = ((float)Random.Range(0, 100) / 100f) < m_CurrLevel.rareObjectsPercentage ? true : false;
        if (isRare)
            return GetRandomRareObject();
        else
            return GetRandomCommonObject();
    }

    private IngameObject GetRandomCommonObject()
    {
        float rand = ((float)Random.Range(0, 100) / 100f);
        float rate = 0f;
        foreach (IngameObbjectRate ingameObbjectRate in m_CurrLevel.commmonObjects)
        {
            rate += ingameObbjectRate.percentage;
            if (rand < rate)
                return ingameObbjectRate.ingameObject;
        }
        return m_CurrLevel.commmonObjects[0].ingameObject;
    }

    private IngameObject GetRandomRareObject()
    {
        float rand = ((float)Random.Range(0, 100) / 100f);
        float rate = 0f;
        foreach (IngameObbjectRate ingameObbjectRate in m_CurrLevel.rareObjects)
        {
            rate += ingameObbjectRate.percentage;
            if (rand < rate)
                return ingameObbjectRate.ingameObject;
        }
        return m_CurrLevel.rareObjects[0].ingameObject;
    }

    public ClearMission GetRandomClearMission()
    {
        m_MissionCount++;
        ClearMission rand = Instantiate(m_CurrLevel.clearMissions[Random.Range(0, m_CurrLevel.clearMissions.Count)]);
        rand.counter *= (int)Mathf.Ceil(m_MissionCount / 3);
        rand.price *= (int)Mathf.Ceil(m_MissionCount / 3);
        return rand;
    }
}
