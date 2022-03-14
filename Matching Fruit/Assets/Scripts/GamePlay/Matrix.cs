using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matrix : MonoBehaviour
{
    public static Matrix instance;

    public enum MatrixState
    {
        None, Block, Chained, Freeze
    }

    public enum ObjectState
    {
        Normal, Falling, Destroyed, UseEffect, Empty
    }

    private int row;
    private int column;
    private GameObject prefab;
    [SerializeField] private GameObject vfx;

    public int Row => row;
    public int Column => column;

    private Object[,] m_Matrix;
    private MatrixState[,] m_MatrixState;
    public ObjectState[,] m_ObjectsState;

    internal ObjectVFX[,] m_MatrixVFX;
    internal Queue<Object> m_EffectQueue;

    private Vector3 m_ObjectSize;
    public Vector3 ObjectSize => m_ObjectSize;
    public bool m_IsBusy = false;
    //private int m_Combo = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        DataManager.instance.SetMatrixData(ref row, ref column, ref prefab);
        m_ObjectSize = prefab.GetComponent<SpriteRenderer>().bounds.size;
        m_Matrix = new Object[row, column];
        m_MatrixState = new MatrixState[row, column];
        m_ObjectsState = new ObjectState[row, column];
        m_MatrixVFX = new ObjectVFX[row, column];
        m_EffectQueue = new Queue<Object>();
        StartCoroutine(InitMap());
    }

    private IEnumerator InitMap()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // init object
                Vector3 pos = transform.position;
                pos.x += j * m_ObjectSize.x - m_ObjectSize.x * (column - 1) / 2;
                pos.y += i * m_ObjectSize.y - m_ObjectSize.y * (row - 1) / 2;
                m_Matrix[i, j] = Instantiate(prefab, pos, Quaternion.identity).GetComponent<Object>();
                m_Matrix[i, j].transform.parent = transform;
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomCommonObject());
                m_Matrix[i, j].m_MatrixPosition = pos;
                m_Matrix[i, j].m_MatrixIndex = new Vector2Int(i, j);
                // init falling
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].Position = pos;
            }
        }

        // init object vfx
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                Vector3 pos = transform.position;
                pos.x += j * m_ObjectSize.x  - m_ObjectSize.x * (column - 1) / 2;
                pos.y += i * m_ObjectSize.y - m_ObjectSize.y * (row - 1) / 2;
                m_MatrixVFX[i, j] = Instantiate(vfx, pos, Quaternion.identity).GetComponent<ObjectVFX>();
                m_MatrixVFX[i, j].transform.parent = transform;
            }
        }

        //init state
        DataManager.instance.SetMatrixState(ref m_MatrixState, ref m_Matrix);
        InitObjectsState(true);

        while (CheckFallingObjects())
            yield return null;

        Mechanic.instance.CheckMatching(m_Matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(m_Matrix));
    }

    public void InitObjectsState(bool isResetAll)
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                if (!isResetAll && m_ObjectsState[i, j] == ObjectState.Empty)
                    continue;
                m_ObjectsState[i, j] = ObjectState.Falling;
            }
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
    public IEnumerator ResetMatrix()
    {
        for (int i = 0; i < row; i++)
            for (int j = 0; j < column; j++)
            {
                if (m_MatrixState[i, j] == MatrixState.None)
                    m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomCommonObject());

                m_Matrix[i, j].m_Velocity = Vector2.zero;

                // init falling
                Vector3 pos = m_Matrix[i, j].m_MatrixPosition;
                pos.y += row * m_ObjectSize.y;
                m_Matrix[i, j].Position = pos;
            }

        //init state
        InitObjectsState(true);

        while (CheckFallingObjects())
            yield return null;
        Mechanic.instance.CheckMatching(m_Matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(m_Matrix));
    }

    // for Object (from falling to normal)
    public void UpdateFallingObject(Vector2Int pos, bool isFalling)
    {
        if (isFalling)
            m_ObjectsState[pos.x, pos.y] = ObjectState.Falling;
        else if (m_ObjectsState[pos.x, pos.y] == ObjectState.Falling)
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX_FALLING_OBJECT);
            m_ObjectsState[pos.x, pos.y] = ObjectState.Normal;
        }
    }

    // Check if an object is destroyed or use effect
    public bool CheckDestroyObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_ObjectsState[i, j] == ObjectState.Destroyed
            || m_ObjectsState[i, j] == ObjectState.UseEffect)
            return true;
        return false;
    }

    // Check if an object is selectable
    public bool CheckSelectableObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_MatrixState[i, j] == MatrixState.None && m_ObjectsState[i, j] != ObjectState.Empty)
                return true;
        return false;
    }

    // Check if an object restricted movement
    public bool CheckIfObjectCanFall(int i, int j)
    {
        if (m_MatrixState[i, j] != MatrixState.None)
            return false;
        return true;
    }

    // Check if an object is empty
    public bool CheckEmptyObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            return m_ObjectsState[i, j] == ObjectState.Empty;
        return false;
    }

    // Set an object empty
    public void SetEmptyObject(int i, int j)
    {
        m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetEmptyObject());
        m_Matrix[i, j].ResetState();
        m_ObjectsState[i, j] = ObjectState.Empty;
    }

    public void UnlockIfFreeze(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_MatrixState[i, j] == MatrixState.Freeze)
            {
                m_Matrix[i, j].SetStateNormal();
                m_MatrixState[i, j] = MatrixState.None;
                ResetObjectState(i, j);
            }
    }

    public void SafeDestroyVFX(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveDestroy();
    }

    public void SafeBombVFX(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveBomb();
    }

    public void SafeLightningVFX(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveLightning();
    }

    public void SafeClockVFX(int i, int j, string text)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveClock(text);
    }

    public void SafeRainbowVFX(int i, int j, int targetX, int targetY)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveRainbow(targetX, targetY);
    }

    public void SafeHammerVFX(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            m_MatrixVFX[i, j].ActiveHammer();
    }

    // Set an object to be destroyed or use effect
    public void SafeDestroyObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            if (m_ObjectsState[i, j] == ObjectState.Empty || m_MatrixState[i, j] == MatrixState.Block)
                return;

            // unlock freeze
            if (m_MatrixState[i, j] == MatrixState.Freeze)
            {
                UnlockIfFreeze(i, j);
                return;
            }

            // unlock nearby freezing objects
            if (m_Matrix[i, j].Properties.isRare == false)
            { 
                                    UnlockIfFreeze(i + 1, j);
            UnlockIfFreeze(i, j - 1);                           ; UnlockIfFreeze(i, j + 1);
                                    UnlockIfFreeze(i - 1, j);
            }

            // unlock chain
            if (m_MatrixState[i, j] == MatrixState.Chained)
            {
                m_Matrix[i, j].SetStateNormal();
                m_MatrixState[i, j] = MatrixState.None;
                ResetObjectState(i, j);
                return;
            }

            m_MatrixVFX[i, j].ActiveDestroy();
            if (m_Matrix[i, j].Properties.isRare == false)
            {
                // audio
                AudioManager.instance.PlaySFX(AudioManager.SFX_DESTROY);
                m_ObjectsState[i, j] = ObjectState.Destroyed;
            }
            else
            {
                if (m_ObjectsState[i, j] != ObjectState.UseEffect)
                {
                    m_EffectQueue.Enqueue(m_Matrix[i, j]);
                    m_ObjectsState[i, j] = ObjectState.UseEffect;
                }
            }
        }
    }

    // Set an object to be destroyed or use effect
    public void SafeCompleteDestroyObject(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            m_ObjectsState[i, j] = ObjectState.Empty;
            m_Matrix[i, j].SetStateNormal();
            m_MatrixState[i, j] = MatrixState.None;
            m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetEmptyObject());
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
    public bool CheckMatchWithStateNone(int i, int j, IngameObject.ObjectType type)
    {
        if (type == IngameObject.ObjectType.Empty)
            return false;
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            if (CheckSelectableObject(i, j))
                return m_Matrix[i, j].Properties.type == type;
        }
        return false;
    }

    // for Mechanic
    public bool CheckMatch(int i, int j, IngameObject.ObjectType type)
    {
        if (type == IngameObject.ObjectType.Empty)
            return false;
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            if (m_MatrixState[i, j] == MatrixState.None || m_MatrixState[i, j] == MatrixState.Chained)
                return m_Matrix[i, j].Properties.type == type;
        }
        return false;
    }

    // for Mechanic
    public IEnumerator Swap(Object first, Object second)
    {
        StartCoroutine(first.MoveTo(second.m_MatrixPosition, 0.5f));
        yield return second.MoveTo(first.m_MatrixPosition, 0.5f);

        first.Position = first.m_MatrixPosition;
        second.Position = second.m_MatrixPosition;
        IngameObject tmp = first.Properties;
        first.SetObjectProperties(second.Properties);
        second.SetObjectProperties(tmp);
    } 

    public void SetStateBlock(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
        {
            AudioManager.instance.PlaySFX(AudioManager.SFX_BLOCK);
            m_MatrixState[i, j] = Matrix.MatrixState.Block;
            m_Matrix[i, j].SetStateBlock();
        }
    }

    public void FreeStateBlock(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_MatrixState[i, j] == Matrix.MatrixState.Block)
            {
                m_MatrixState[i, j] = Matrix.MatrixState.None;
                m_Matrix[i, j].SetStateNormal();
                m_Matrix[i, j].SetObjectProperties(DataManager.instance.GetEmptyObject());
                m_ObjectsState[i, j] = ObjectState.Empty;
            }
    }

    public void SetStateChained(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_MatrixState[i, j] == Matrix.MatrixState.None && m_ObjectsState[i, j] != ObjectState.Empty)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX_CHAIN);
                m_MatrixState[i, j] = Matrix.MatrixState.Chained;
                m_Matrix[i, j].SetStateChained();
            }
    }

    public void SetStateFreeze(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            if (m_MatrixState[i, j] == Matrix.MatrixState.None && m_ObjectsState[i, j] != ObjectState.Empty)
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX_ICE);
                m_MatrixState[i, j] = Matrix.MatrixState.Freeze;
                m_Matrix[i, j].SetStateFreeze();
            }
    }

    public void SetWarnSafe(int i, int j)
    {
        if ((i > -1 && i < row) && (j > -1 && j < column))
            StartCoroutine(m_Matrix[i, j].SetWarn());
    }
}
