using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matrix : MonoBehaviour
{
    public static Matrix instance;

    // 4 extra row unsused when calculate
    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private GameObject prefab;

    private Object[,] m_Matrix;
    private int[,] m_ExistingObjects;
    private Vector3 m_ObjectSize;

    public bool m_CanMatching = true;
    private bool m_IsInCombo = false;
    private int m_Combo = 0;

    private static Object firstSelected = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        m_ObjectSize = prefab.GetComponent<SpriteRenderer>().bounds.size;
        m_Matrix = new Object[row, column];
        m_ExistingObjects = new int[row, column];
        InitMap();
        InitExistingObjectsMatrix();

        StartCoroutine(StartMatchCombo());
    }

    public void ResetMatrix()
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomCommonObject());

        StartCoroutine(StartMatchCombo());
    }

    private void InitMap()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector3 pos = transform.position;
                pos.x += j * (m_ObjectSize.x + 1);
                pos.y += i * m_ObjectSize.y;
                m_Matrix[i, j] = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Object>();
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomCommonObject());
            }
        } 
    }

    private void InitExistingObjectsMatrix()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                m_ExistingObjects[i, j] = 1;
            }
        }
    }

    private bool CheckCanContinue()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column - 1; j++)
            {
                Swap(m_Matrix[i, j], m_Matrix[i, j + 1]);
                if (CheckMatching())
                {
                    Swap(m_Matrix[i, j], m_Matrix[i, j + 1]);

                    // Reset Existing Check Matrix
                    InitExistingObjectsMatrix();
                    return true;
                }
                Swap(m_Matrix[i, j], m_Matrix[i, j + 1]);
            }
        }
        for (int j = 0; j < column; j++)
        {
            for (int i = 0; i < row - 1; i++)
            {
                Swap(m_Matrix[i + 1, j], m_Matrix[i, j]);
                if (CheckMatching())
                {
                    Swap(m_Matrix[i + 1, j], m_Matrix[i, j]);

                    // Reset Existing Check Matrix
                    InitExistingObjectsMatrix();
                    return true;
                }
                Swap(m_Matrix[i + 1, j], m_Matrix[i, j]);
            }
        }

        // Reset Existing Check Matrix
        InitExistingObjectsMatrix();
        return false;
    }

    public void ObjectClicked(Object objectClicked)
    {
        if (m_IsInCombo)
            return;

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
                if (CheckSwap(firstSelected, objectClicked))
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

    private bool CheckSwap(Object first, Object second)
    {
        // Get pos
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

        //Debug.Log(firstPos + "--" + secondPos);

        // Check if is swapable
        if (firstPos.x == secondPos.x && Mathf.Abs(firstPos.y - secondPos.y) == 1 ||
            firstPos.y == secondPos.y && Mathf.Abs(firstPos.x - secondPos.x) == 1)
        {
            Swap(m_Matrix[firstPos.x, firstPos.y], m_Matrix[secondPos.x, secondPos.y]);

            if (CheckMatching())
            {
                // minus 1 turn
                GameManager.instance.DecreaseTurn();
                // start match chain
                StartCoroutine(StartMatchCombo());
            }
            else
            {
                Swap(m_Matrix[secondPos.x, secondPos.y], m_Matrix[firstPos.x, firstPos.y]);
                Debug.Log("Swap lam j bn ?");
            }

            return true;
        }

        return false;
    }

    private IEnumerator StartMatchCombo()
    {
        m_IsInCombo = true;

        while (CheckMatching())
        {
            if (m_CanMatching)
            {
                m_Combo++;
                GameManager.instance.UpdateScores(UpdateMatrix(), m_Combo);
                
                Debug.Log("Combo: " + m_Combo);
            }
            yield return new WaitForSeconds(0.5f);
        }

        if (!CheckCanContinue())
        {
            GameManager.instance.OutOfMove();
        }

        m_Combo = 0;
        m_IsInCombo = false;
    }

    private void Swap(Object first, Object second)
    {
        //// Swap in matrix
        //(m_Matrix[firstPos.x, firstPos.y], m_Matrix[secondPos.x, secondPos.y]) =
        //    (m_Matrix[secondPos.x, secondPos.y], m_Matrix[firstPos.x, firstPos.y]);
        //// Swap visually
        //(first.transform.position, second.transform.position) =
        //    (second.transform.position, first.transform.position);

        IngameObject tmp = first.properties;
        first.SetObjectProperties(second.properties);
        second.SetObjectProperties(tmp);
    }

    private bool CheckMatching()
    {
        bool isMatch = false;

        // Horizontal
        for (int i = 0; i < row; i++)
        {
            int[] checkList = new int[column];
            checkList[0] = 1;
            
            for (int j = 1; j < column; j++)
            {
                if (m_Matrix[i, j - 1].properties.type == m_Matrix[i, j].properties.type)
                    checkList[j] = checkList[j - 1] + 1;
                else
                    checkList[j] = 1; 
            }
            int count = 0;

            for (int j = column - 1; j > -1; j--)
            {
                if (count > 0)
                {
                    m_ExistingObjects[i, j] = 0;
                    count--;
                }
                else if (checkList[j] >= 3)
                {
                    isMatch = true;
                    m_ExistingObjects[i, j] = 0;
                    count = checkList[j] - 1;
                }               
            }
        }

        // Vertical
        for (int j = 0; j < column; j++)
        {
            int[] checkList = new int[column];
            checkList[0] = 1;

            for (int i = 1; i < row; i++)
            {
                if (m_Matrix[i - 1, j].properties.type == m_Matrix[i, j].properties.type)
                    checkList[i] = checkList[i - 1] + 1;
                else
                    checkList[i] = 1; 
            }

            int count = 0;
            for (int i = row - 1; i > -1; i--)
            {
                if (count > 0)
                {
                    m_ExistingObjects[i, j] = 0;
                    count--;
                }
                else if (checkList[i] >= 3)
                {
                    isMatch = true;
                    m_ExistingObjects[i, j] = 0;
                    count = checkList[i] - 1;
                }
            }
        }

        return isMatch;
    }

    private int UpdateMatrix()
    {
        int matchedObjectcount = 0;

        int[] columnQueue = new int[column];
        Vector3[,] newPoses = new Vector3[row, column];
        for (int i = 0; i < column; i++)
            columnQueue[i] = 0;

        // update mission
        // TODO: consider using Observer pattern
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_ExistingObjects[i, j] == 0)
                {
                    GameManager.instance.UpdateClearMission(m_Matrix[i, j].properties);
                    matchedObjectcount++;
                }         
            }
        }

        // Update Properties
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_ExistingObjects[i, j] == 0)
                {
                    int r = i + 1;
                    while (r < row)
                    {
                        if (m_ExistingObjects[r, j] != 0)
                        {
                            m_Matrix[i, j].SetObjectProperties(m_Matrix[r, j].properties);
                            newPoses[i, j] = m_Matrix[r, j].transform.position;
                            m_ExistingObjects[i, j] = 1;
                            m_ExistingObjects[r, j] = 0;
                            break;
                        }
                        r++;
                    }
                    if (m_ExistingObjects[i, j] == 0)
                    {
                        m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomCommonObject());
                        Vector3 pos = transform.position;
                        pos.x += j * (m_ObjectSize.x + 1);
                        pos.y += (row + columnQueue[j] + 1) * m_ObjectSize.y;
                        newPoses[i, j] = pos;
                        columnQueue[j]++;
                    }
                }
                else
                    newPoses[i, j] = m_Matrix[i, j].transform.position;
            }
        }

        // Update Position
        for (int i = row - 1; i > -1; i--)
        {
            for (int j = 0; j < column; j++)
            {
                m_Matrix[i, j].transform.position = newPoses[i, j];
            }
        }

        // Reset Existing Check Matrix
        InitExistingObjectsMatrix();

        return matchedObjectcount;
    }
}
