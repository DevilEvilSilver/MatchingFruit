using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int[,] m_Matrix;
    private List<SpawnObjects> m_SpawnList;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Matrix = new int[15, 15];
        m_SpawnList = new List<SpawnObjects>();
        m_SpawnList.AddRange(gameObject.GetComponentsInChildren<SpawnObjects>());
        m_SpawnList.Sort(delegate (SpawnObjects o1, SpawnObjects o2) 
        {
            return o1.transform.position.x.CompareTo(o2.transform.position.x); 
        });
        InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitMap()
    {

    }
}
