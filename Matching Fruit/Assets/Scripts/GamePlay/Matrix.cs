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
    [SerializeField] private float rareObjectPercentage;

    private Object[,] m_Matrix;
    private int[,] m_ExistingObjects;
    private Vector3 m_ObjectSize;

    private bool[,] m_FallingObjects;
    private bool m_IsInCombo = false;
    private int m_Combo = 0;

    private static Object firstSelected = null;
    private KeyValuePair<Vector2Int, Vector2Int>? currHintedPair = null;

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
        m_FallingObjects = new bool[row, column];
        InitMap();
        InitObjectsObserverMatrix();

        StartCoroutine(StartMatchCombo());
    }

    public void ResetMatrix()
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                m_Matrix[i, j].SetObjectProperties(GachaObject());
                m_FallingObjects[i, j] = true;
                m_Matrix[i, j].m_Velocity = Vector2.zero;
                Vector3 pos = m_Matrix[i, j].m_MatrixPosition;
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].transform.position = pos;
            }

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
                m_Matrix[i, j].SetObjectProperties(GachaObject());
                m_Matrix[i, j].m_MatrixPosition = pos;
                m_Matrix[i, j].m_MatrixIndex = new Vector2Int(i, j);
                // init falling
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].transform.position = pos;
            }
        }
    }

    private IngameObject GachaObject()
    {
        bool isRare = ((float)Random.Range(0, 100) / 100f) < rareObjectPercentage ? true : false;
        if (isRare)
            return DataManager.instance.GetRandomRareObject();
        else
            return DataManager.instance.GetRandomCommonObject();
    }

    private void InitObjectsObserverMatrix()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                m_ExistingObjects[i, j] = 1;
                m_FallingObjects[i, j] = true;
            }
        }
    }

    public void UpdateFallingObject(Vector2Int pos, bool isFalling)
    {
        m_FallingObjects[pos.x, pos.y] = isFalling;
    }

    private bool CheckFallingObject()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_FallingObjects[i, j] == true)
                    return true;
            }
        }
        return false;
    }

    // Will return incorrect result while in combo (Only check when no more matching occure)
    public KeyValuePair<Vector2Int, Vector2Int>? CheckCanContinue()
    {
        // Need to return !null while in combo
        //if (m_IsInCombo)
        //    return null;

        // pattern
        // x ? x x ? x
        // ? x o o x ?
        // x ? x x ? x
        // Horizontal
        for (int i = 0; i < row; i++)
        {
            for (int j = 1; j < column; j++)
            {
                if (m_Matrix[i, j - 1].Properties.type == m_Matrix[i, j].Properties.type)
                {
                    // x x o
                    // o o x
                    // x x x
                    if (CheckMatchSafely(i + 1, j + 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i + 1, j + 1));
                    }
                    // x x x
                    // o o x
                    // x x o
                    if (CheckMatchSafely(i - 1, j + 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i - 1, j + 1));
                    }
                    // o o x o
                    if (CheckMatchSafely(i, j + 2, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i, j + 2));
                    }
                    // o x x 
                    // x o o 
                    // x x x 
                    if (CheckMatchSafely(i + 1, j - 2, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i + 1, j - 2));
                    }
                    // x x x
                    // x o o
                    // o x x
                    if (CheckMatchSafely(i - 1, j - 2, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i - 1, j - 2));
                    }
                    // o x o o
                    if (CheckMatchSafely(i, j - 3, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i, j - 3));
                    }
                }
            }
        }
        // Vertical
        for (int j = 0; j < column; j++)
        {
            for (int i = 1; i < row; i++)
            {
                if (m_Matrix[i - 1, j].Properties.type == m_Matrix[i, j].Properties.type)
                {
                    // x x o
                    // x o x
                    // x o x
                    if (CheckMatchSafely(i + 1, j + 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 1, j + 1));
                    }
                    // o x x
                    // x o x
                    // x o x
                    if (CheckMatchSafely(i + 1, j - 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 1, j - 1));
                    }
                    // o
                    // x
                    // o
                    // o
                    if (CheckMatchSafely(i + 2, j, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 2, j));
                    }
                    // x o x
                    // x o x
                    // x x o
                    if (CheckMatchSafely(i - 2, j + 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 2, j), new Vector2Int(i - 2, j + 1));
                    }
                    // x o x
                    // x o x
                    // o x x
                    if (CheckMatchSafely(i - 2, j - 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 2, j), new Vector2Int(i - 2, j - 1));
                    }
                    // o
                    // o
                    // x
                    // o
                    if (CheckMatchSafely(i - 3, j, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 2, j), new Vector2Int(i - 3, j));
                    }
                }
            }
        }

        // pattern
        // x ? x 
        // o x o 
        // x ? x 
        // Horizontal
        for (int i = 0; i < row; i++)
        {
            for (int j = 2; j < column; j++)
            {
                if (m_Matrix[i, j - 2].Properties.type == m_Matrix[i, j].Properties.type)
                {
                    // x o x
                    // o x o
                    // x x x
                    if (CheckMatchSafely(i + 1, j - 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 1), new Vector2Int(i + 1, j - 1));
                    }
                    // x x x
                    // o x o
                    // x o x
                    if (CheckMatchSafely(i - 1, j - 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 1), new Vector2Int(i - 1, j - 1));
                    }
                }
            }
        }
        // Vertical
        for (int j = 0; j < column; j++)
        {
            for (int i = 2; i < row; i++)
            {
                if (m_Matrix[i - 2, j].Properties.type == m_Matrix[i, j].Properties.type)
                {
                    // x o x
                    // o x x
                    // x o x
                    if (CheckMatchSafely(i - 1, j - 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 1, j), new Vector2Int(i - 1, j - 1));
                    }
                    // x o x
                    // x x o
                    // x o x
                    if (CheckMatchSafely(i - 1, j + 1, m_Matrix[i, j].Properties.type))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 1, j), new Vector2Int(i - 1, j + 1));
                    }
                }
            }
        }

        return null;
    }

    private bool CheckMatchSafely(int i, int j, IngameObject.ObjectType type)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            return m_Matrix[i, j].Properties.type == type;
        return false;
    }

    public void ObjectClicked(Object objectClicked)
    {
        // Cant select when in combo chain & end game
        if (m_IsInCombo
            || GameManager.instance.m_IsEndGame
            || m_FallingObjects[objectClicked.m_MatrixIndex.x, objectClicked.m_MatrixIndex.y]
            || !GameManager.instance.CheckTurns())
            return;

        // Unselect
        if (firstSelected == objectClicked)
        {
            firstSelected.SetSelected(false);
            firstSelected = null;
            return;
        }
        else
        {
            // Select
            if (firstSelected == null)
            {
                firstSelected = objectClicked;
                firstSelected.SetSelected(true);
                return;
            }
            // Swap
            else
            {
                StartCoroutine(CheckSwap(firstSelected.m_MatrixIndex, objectClicked.m_MatrixIndex));
                firstSelected.SetSelected(false);
                firstSelected = null;
            }
        }
    }

    private IEnumerator CheckSwap(Vector2Int first, Vector2Int second)
    {
        m_IsInCombo = true;

        // Check if is swapable
        if (first.x == second.x && Mathf.Abs(first.y - second.y) == 1 ||
            first.y == second.y && Mathf.Abs(first.x - second.x) == 1)
        {
            yield return Swap(m_Matrix[first.x, first.y], m_Matrix[second.x, second.y]);

            if (CheckMatching() || m_Matrix[first.x, first.y].Properties.isRare || m_Matrix[second.x, second.y].Properties.isRare)
            {
                // start rare effect
                if (m_Matrix[first.x, first.y].Properties.isRare)
                    UseRareObject(m_Matrix[first.x, first.y], m_Matrix[second.x, second.y]);
                if (m_Matrix[second.x, second.y].Properties.isRare)
                    UseRareObject(m_Matrix[second.x, second.y], m_Matrix[first.x, first.y]);

                // start match chain
                yield return StartMatchCombo();

                // minus 1 turn
                GameManager.instance.DecreaseTurn();
            }
            else
            {
                StartCoroutine(m_Matrix[first.x, first.y].SetWarn());
                yield return m_Matrix[second.x, second.y].SetWarn();

                yield return Swap(m_Matrix[second.x, second.y], m_Matrix[first.x, first.y]);
            }
        }

        m_IsInCombo = false;
    }

    private IEnumerator StartMatchCombo()
    {
        while (CheckMatching())
        {
            if (!CheckFallingObject())
            {
                m_Combo++;
                GameManager.instance.UpdateScores(UpdateMatrix(), m_Combo);
            }
            yield return null;
        }

        if (CheckCanContinue() == null)
        {
            StartCoroutine(GameManager.instance.OutOfMove());
        }

        m_Combo = 0;
    }

    private IEnumerator Swap(Object first, Object second)
    {
        StartCoroutine(first.MoveTo(second.m_MatrixPosition, 0.5f));
        yield return second.MoveTo(first.m_MatrixPosition, 0.5f);

        first.transform.position = first.m_MatrixPosition;
        second.transform.position = second.m_MatrixPosition;
        IngameObject tmp = first.Properties;
        first.SetObjectProperties(second.Properties);
        second.SetObjectProperties(tmp);
    }

    public bool CheckMatching()
    {
        bool isMatch = false;

        // Horizontal
        for (int i = 0; i < row; i++)
        {
            int[] checkList = new int[column];
            checkList[0] = 1;
            
            for (int j = 1; j < column; j++)
            {
                if (m_Matrix[i, j - 1].Properties.type == m_Matrix[i, j].Properties.type)
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
                else if (checkList[j] >= 3 && !m_Matrix[i, j].Properties.isRare)
                {
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
                if (m_Matrix[i - 1, j].Properties.type == m_Matrix[i, j].Properties.type)
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
                else if (checkList[i] >= 3 && !m_Matrix[i, j].Properties.isRare)
                {
                    m_ExistingObjects[i, j] = 0;
                    count = checkList[i] - 1;
                }
            }
        }

        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                if (m_ExistingObjects[i, j] == 0)
                {
                    isMatch = true;
                    break;
                }

        return isMatch;
    }

    private int UpdateMatrix()
    {
        int matchedObjectcount = 0;

        int[] columnQueue = new int[column];
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
                    GameManager.instance.UpdateClearMission(m_Matrix[i, j].Properties);
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
                            m_Matrix[i, j].SetObjectProperties(m_Matrix[r, j].Properties);
                            m_Matrix[i, j].transform.position = m_Matrix[r, j].transform.position;
                            m_ExistingObjects[i, j] = 1;
                            m_ExistingObjects[r, j] = 0;
                            break;
                        }
                        r++;
                    }
                    if (m_ExistingObjects[i, j] == 0)
                    {
                        m_Matrix[i, j].SetObjectProperties(GachaObject());
                        Vector3 pos = transform.position;
                        pos.x += j * (m_ObjectSize.x + 1);
                        pos.y += (row + columnQueue[j] + 1) * m_ObjectSize.y;
                        m_Matrix[i, j].transform.position = pos;
                        columnQueue[j]++;
                    }
                }
            }
        }

        // Reset Existing Check Matrix
        InitObjectsObserverMatrix();

        return matchedObjectcount;
    }

    private void UseRareObject(Object rareObj, Object affectedObj)
    {
        switch (rareObj.Properties.type)
        {
            case IngameObject.ObjectType.Rainbow:
                {
                    m_ExistingObjects[rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y] = 0;
                    UseRainbowEffect(affectedObj);
                    break;
                }
            case IngameObject.ObjectType.Bomb:
                {
                    m_ExistingObjects[rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y] = 0;
                    UseBombEffect(rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y);
                    break;
                }
            case IngameObject.ObjectType.Clock:
                {
                    m_ExistingObjects[rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y] = 0;
                    UseClockEffect();
                    break;
                }
        }
    }

    private void UseRainbowEffect(Object obj)
    {
        if (obj.Properties.isRare == false)
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                {
                    if (m_Matrix[i, j].Properties.type == obj.Properties.type)
                        m_ExistingObjects[i, j] = 0;
                }
        }
        else if (obj.Properties.type == IngameObject.ObjectType.Rainbow)
        {
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    m_ExistingObjects[i, j] = 0;
        }
    }

    private void UseBombEffect(int i, int j)
    {
        SafeDeleteObject(i + 1, j - 1); SafeDeleteObject(i + 1, j); SafeDeleteObject(i + 1, j + 1);
        SafeDeleteObject(i, j - 1); SafeDeleteObject(i, j); SafeDeleteObject(i, j + 1);
        SafeDeleteObject(i - 1, j - 1); SafeDeleteObject(i - 1, j); SafeDeleteObject(i - 1, j + 1);
    }
    
    private void SafeDeleteObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_ExistingObjects[i, j] = 0;
    }

    private void UseClockEffect()
    {
        GameManager.instance.AddTime(5f);
    }

    public void SetHintPair(KeyValuePair<Vector2Int, Vector2Int>? pair)
    {
        currHintedPair = pair;
    }

    public void SetHint(bool isHint)
    {
        if (currHintedPair == null)
            return;
        else
        {
            m_Matrix[currHintedPair.Value.Key.x, currHintedPair.Value.Key.y].SetHint(isHint);
            m_Matrix[currHintedPair.Value.Value.x, currHintedPair.Value.Value.y].SetHint(isHint);
        }
    }
}
