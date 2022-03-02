using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private List<IngameObject> m_CommmonObjects = new List<IngameObject>();
    [SerializeField] private List<ClearMission> m_ClearMissions = new List<ClearMission>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IngameObject GetRandomCommonObject()
    {
        return m_CommmonObjects[Random.Range(0, m_CommmonObjects.Count)];
    }

    public ClearMission GetRandomClearMission()
    {
        return Instantiate(m_ClearMissions[Random.Range(0, m_ClearMissions.Count)]);
    }
}
