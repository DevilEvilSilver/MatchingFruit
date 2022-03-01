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
                m_SpawnList[j].AddSpawn(DataManager.instance.GetRandomCommonObject(), new Vector2Int(i ,j));
            }     
        }
    }

    public void SetObjectToMatrix(GameObject ingameObject, Vector2Int pos)
    {
        m_Matrix[pos.x, pos.y] = ingameObject;
    }

    public bool Swap(GameObject first, GameObject second)
    {
        // Check if is swapable
        Vector2Int firstPos = Vector2Int.zero, secondPos = Vector2Int.zero;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_Matrix[i, j] == first)
                    firstPos = new Vector2Int(i, j);
                if (m_Matrix[i, j] == second)
                    secondPos = new Vector2Int(i, j);
            }
        }

        Debug.Log(firstPos );
        Debug.Log( secondPos);

        if (firstPos.x == secondPos.x && Mathf.Abs(firstPos.y - secondPos.y) == 1 ||
            firstPos.y == secondPos.y && Mathf.Abs(firstPos.x - secondPos.x) == 1)
        {
            // Swap in matrix
            (m_Matrix[firstPos.x, firstPos.y], m_Matrix[secondPos.x, secondPos.y]) =
                (m_Matrix[secondPos.x, secondPos.y], m_Matrix[firstPos.x, firstPos.y]);
            // Swap visually
            (first.transform.position, second.transform.position) =
                (second.transform.position, first.transform.position);

            return true;
        }

        return false;
    }
}
