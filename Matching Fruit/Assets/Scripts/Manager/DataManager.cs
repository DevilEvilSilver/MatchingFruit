using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [SerializeField] private List<GameObject> m_CommeonObjects = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public GameObject GetRandomCommonObject()
    {
        return m_CommeonObjects[Random.Range(0, m_CommeonObjects.Count)];
    }
}
