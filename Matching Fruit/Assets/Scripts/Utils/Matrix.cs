using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix : MonoBehaviour
{
    public static Matrix instance;

    // 4 extra row unsused when calculate
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private GameObject prefab;

    private GameObject[,] m_Matrix;
    private List<SpawnObjects> m_SpawnList;
    private static Object firstSelected = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        InitSpawnList();
        InitMap();
    }

    void Update()
    {
        //CheckMatching();
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

    private void InitMap()
    {
        m_Matrix = new GameObject[row, column];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector3 pos = transform.position;
                pos.x += j * 51;
                pos.y += i * 51;
                m_Matrix[i, j] = Instantiate(prefab, pos, Quaternion.identity);
                m_Matrix[i, j].GetComponent<Object>().SetObjectProperties(DataManager.instance.GetRandomCommonObject());
            }
        }
    }

    public void ObjectClicked(Object objectClicked)
    {
        if (firstSelected == objectClicked)
        {
            firstSelected.SetSelected(false);
            firstSelected = null;
            return;
        }
        else
        {
            if (firstSelected == null)
            {
                firstSelected = objectClicked;
                firstSelected.SetSelected(true);
                return;
            }
            else
            {
                objectClicked.SetSelected(true);
                if (Swap(firstSelected.gameObject, objectClicked.gameObject))
                {
                    firstSelected.SetSelected(false);
                    objectClicked.SetSelected(false);
                    firstSelected = null;
                }
                else
                {
                    firstSelected.SetSelected(false);
                    firstSelected = objectClicked;
                }
            }
        }
    }

    private bool Swap(GameObject first, GameObject second)
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

        Debug.Log(firstPos);
        Debug.Log(secondPos);

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

    public void SetObjectToMatrix(GameObject ingameObject, Vector2Int pos)
    {
        //m_Matrix[pos.x, pos.y] = ingameObject;
    }

    private void CheckMatching()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                int count = 0;
                // Check Vertical
                while (i + count < row - 1)
                {
                    count++;
                    if (m_Matrix[i, j].tag == m_Matrix[i + count, j].tag)
                    {
                        continue;
                    }
                    else
                        break;
                }
                if (count >= 3)
                {
                    SetMatching(new Vector2Int(i, j), count, true);
                    return;
                }

                count = 0;
                // Check Horizontal
                while (j + count < column - 1)
                {
                    count++;
                    if (m_Matrix[i, j].tag == m_Matrix[i, j + count].tag)
                    {
                        continue;
                    }
                    else
                        break;
                }
                if (count >= 3)
                {
                    SetMatching(new Vector2Int(i, j), count, false);
                    return;
                }
            }
        }
    }

    private void SetMatching(Vector2Int pos, int count, bool isVertival)
    {
        if (isVertival)
        {
            Debug.Log("doc");
            // Destroy
            for (int i = 0; i < count; i++)
            {
                Destroy(m_Matrix[pos.x + i, pos.y]);
            }
            // Update
            for (int i = pos.x; i < row; i++)
            {
                if (i + count < row)
                    m_Matrix[i, pos.y] = m_Matrix[i + count, pos.y];
                else { }
                    //m_SpawnList[pos.y].AddSpawn(DataManager.instance.GetRandomCommonObject(), new Vector2Int(i, pos.y));
            }
        }
        else
        {
            Debug.Log("ngang");
            // Destroy
            for (int j = 0; j < count; j++)
            {
                Destroy(m_Matrix[pos.x, pos.y + j]);
            }
            // Update
            for (int j = pos.y; j < pos.y + count; j++)
            {
                for (int i = pos.x; i < row - 1; i++)
                {
                    m_Matrix[i, j] = m_Matrix[i + 1, j];
                }
                //m_SpawnList[j].AddSpawn(DataManager.instance.GetRandomCommonObject(), new Vector2Int(row - 1, j));
            }
        }
    }
}
