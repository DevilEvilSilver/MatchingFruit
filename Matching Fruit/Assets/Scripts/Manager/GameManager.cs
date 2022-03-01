using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int row;
    [SerializeField] private int column;

    private GameObject[,] m_Matrix;
    private List<SpawnObjects> m_SpawnList;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSpawnList();
        InitMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitSpawnList()
    {
        m_SpawnList = new List<SpawnObjects>();
        m_SpawnList.AddRange(gameObject.GetComponentsInChildren<SpawnObjects>());
        m_SpawnList.Sort(delegate (SpawnObjects o1, SpawnObjects o2)
        {
            return o1.transform.position.x.CompareTo(o2.transform.position.x);
        });
    }

    public void InitMap()
    {
        m_Matrix = new GameObject[row, column];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                m_Matrix[i, j] = DataManager.instance.GetRandomCommonObject();
                m_SpawnList[j].AddSpawn(m_Matrix[i, j]);
            }     
        }
    }
}
