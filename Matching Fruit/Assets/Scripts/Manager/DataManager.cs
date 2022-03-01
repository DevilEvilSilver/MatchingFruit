using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private List<IngameObject> m_CommmonObjects = new List<IngameObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IngameObject GetRandomCommonObject()
    {
        return m_CommmonObjects[Random.Range(0, m_CommmonObjects.Count)];
    }
}
