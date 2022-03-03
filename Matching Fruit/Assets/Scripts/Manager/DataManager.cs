using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private List<IngameObject> m_CommmonObjects = new List<IngameObject>();
    [SerializeField] private List<IngameObject> m_RareObjects = new List<IngameObject>();
    [SerializeField] private List<ClearMission> m_ClearMissions = new List<ClearMission>();

    private int m_MissionCount = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IngameObject GetRandomCommonObject()
    {
        return m_CommmonObjects[Random.Range(0, m_CommmonObjects.Count)];
    }

    public IngameObject GetRandomRareObject()
    {
        return m_RareObjects[Random.Range(0, m_RareObjects.Count)];
    }

    public ClearMission GetRandomClearMission()
    {
        m_MissionCount++;
        ClearMission rand = Instantiate(m_ClearMissions[Random.Range(0, m_ClearMissions.Count)]);
        rand.counter *= (int)Mathf.Ceil(m_MissionCount / 3);
        rand.price *= (int)Mathf.Ceil(m_MissionCount / 3);
        return rand;
    }
}
