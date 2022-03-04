using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matrix : MonoBehaviour
{
    public static Matrix instance;

    public enum ObjectState
    { 
        Normal, Falling, Destroyed, UseEffect, DoneEffect
    }

    [SerializeField] private int row;
    [SerializeField] private int column;
    [SerializeField] private GameObject prefab;
    [SerializeField] private float rareObjectPercentage;

    public int Row => row;
    public int Column => column;

    private Object[,] m_Matrix;
    private ObjectState[,] m_ObjectsState;
    
    private Vector3 m_ObjectSize;
    public Vector3 ObjectSize => m_ObjectSize;
    private bool m_IsBusy = false;
    //private int m_Combo = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        m_ObjectSize = prefab.GetComponent<SpriteRenderer>().bounds.size;
        m_Matrix = new Object[row, column];
        m_ObjectsState = new ObjectState[row, column];
        InitMap();  
    }

    private void InitMap()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // init object
                Vector3 pos = transform.position;
                pos.x += j * (m_ObjectSize.x + 1);
                pos.y += i * m_ObjectSize.y;
                m_Matrix[i, j] = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Object>();
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomObject());
                m_Matrix[i, j].m_MatrixPosition = pos;
                m_Matrix[i, j].m_MatrixIndex = new Vector2Int(i, j);
                // init falling
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].transform.position = pos;
            }
        }

        //init state
        InitObjectsState();

        StartCoroutine(Mechanic.instance.StartMatchCombo(m_Matrix));
    }

    public void InitObjectsState()
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
                m_ObjectsState[i, j] = ObjectState.Falling;
    }

    public Object[,] GetMatrix()
    {
        return m_Matrix;
    }

    // HAVE TO USE LOCK MATRIX & FREE MATRIX AS A PAIR ---- DO NOT USE IN CHILD METHOD OF SOMETHING THAT ALREADY USE THIS
    public Object[,] LockMatrix()
    {
        if (!m_IsBusy)
        {
            m_IsBusy = true;
            return m_Matrix;
        }  
        else
            return null;
    }

    public void FreeMatrix()
    {
        m_IsBusy = false;
    }
    // ---------------------------------------------------------------------------------------------------------

    public bool IsBusy()
    {
        return m_IsBusy;
    }

    // for gamemanager and item
    public void ResetMatrix()
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomObject());
                m_Matrix[i, j].m_Velocity = Vector2.zero;
                
                // init falling
                Vector3 pos = m_Matrix[i, j].m_MatrixPosition;
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].transform.position = pos;
            }

        //init state
        InitObjectsState();

        StartCoroutine(Mechanic.instance.StartMatchCombo(m_Matrix));
    }

    // for Object 
    public void UpdateFallingObject(Vector2Int pos, bool isFalling)
    {
        if (isFalling)
            m_ObjectsState[pos.x, pos.y] = ObjectState.Falling;
        else
            m_ObjectsState[pos.x, pos.y] = ObjectState.Normal;
    }

    // Check if an object is destroyed or use effect
    public bool CheckDestroyObject(int i, int j)
    {
        if (m_ObjectsState[i, j] == ObjectState.Destroyed || m_ObjectsState[i, j] ==  ObjectState.UseEffect || m_ObjectsState[i, j] == ObjectState.DoneEffect)
            return true;
        return false;
    }

    // Set an object to be destroyed or use effect
    public void SafeDestroyObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            if (m_Matrix[i, j].Properties.isRare == false)
                m_ObjectsState[i, j] = ObjectState.Destroyed;
            else
            {
                if (m_ObjectsState[i, j] == ObjectState.UseEffect)
                    m_ObjectsState[i, j] = ObjectState.DoneEffect;
                else if (m_ObjectsState[i, j] != ObjectState.DoneEffect)
                    m_ObjectsState[i, j] = ObjectState.UseEffect;
            }
        }
    }

    public bool CheckInEffectObject()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_ObjectsState[i, j] == ObjectState.UseEffect)
                    return true;
            }
        }
        return false;
    }

    // Set an object to normal state
    public void ResetObjectState(int i, int j)
    {
        m_ObjectsState[i, j] = ObjectState.Falling;
        m_Matrix[i, j].m_Velocity = Vector2.zero;
    }

    // Check if there is any falling object 
    public bool CheckFallingObjects()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                if (m_ObjectsState[i, j] == ObjectState.Falling)
                    return true;
            }
        }
        return false;
    }

    // for Mechanic
    public bool CheckMatchSafely(int i, int j, IngameObject.ObjectType type)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            return m_Matrix[i, j].Properties.type == type;
        return false;
    }

    // for Mechanic
    public IEnumerator Swap(Object first, Object second)
    {
        StartCoroutine(first.MoveTo(second.m_MatrixPosition, 0.5f));
        yield return second.MoveTo(first.m_MatrixPosition, 0.5f);

        first.transform.position = first.m_MatrixPosition;
        second.transform.position = second.m_MatrixPosition;
        IngameObject tmp = first.Properties;
        first.SetObjectProperties(second.Properties);
        second.SetObjectProperties(tmp);
    } 
}
