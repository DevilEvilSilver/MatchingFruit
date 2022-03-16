using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    public static Mechanic instance;

    private Object firstSelected = null;
    private bool isHammering = false;
    private KeyValuePair<Vector2Int, Vector2Int>? currHintedPair = null;
    internal bool IsHammering => isHammering;

    public void Unselect()
    {
        if (firstSelected != null)
            firstSelected.SetSelected(false);
        firstSelected = null;
    }

    public void BuyHammer()
    {
        isHammering = true;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Will return incorrect result while in combo (Only check when no more matching occure)
    public KeyValuePair<Vector2Int, Vector2Int>? CheckCanContinue(Object[,] matrix)
    {
        // Check for rare object
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            for (int j = 0; j < Matrix.instance.Column; j++)
            {
                if (matrix[i, j].Properties.isRare && Matrix.instance.CheckSelectableObject(i, j))
                {
                    if (Matrix.instance.CheckSelectableObject(i, j + 1))
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j), new Vector2Int(i, j + 1));
                    else if (Matrix.instance.CheckSelectableObject(i, j - 1))
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j), new Vector2Int(i, j - 1));
                    else if (Matrix.instance.CheckSelectableObject(i - 1, j))
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j), new Vector2Int(i - 1, j));
                    else if (Matrix.instance.CheckSelectableObject(i + 1, j))
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j), new Vector2Int(i + 1, j));
                }
            }
        }

        // pattern
        // x ? x x ? x
        // ? x o o x ?
        // x ? x x ? x
        // Horizontal
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            for (int j = 1; j < Matrix.instance.Column; j++)
            {
                if (Matrix.instance.CheckMatch(i, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 1) && Matrix.instance.CheckSelectableObject(i, j))
                {
                    // x x o
                    // o o x
                    // x x x
                    if (Matrix.instance.CheckMatchWithStateNone(i + 1, j + 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j + 1))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i + 1, j + 1));
                    }
                    // x x x
                    // o o x
                    // x x o
                    if (Matrix.instance.CheckMatchWithStateNone(i - 1, j + 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j + 1))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i - 1, j + 1));
                    }
                    // o o x o
                    if (Matrix.instance.CheckMatchWithStateNone(i, j + 2, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j + 1))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j + 1), new Vector2Int(i, j + 2));
                    }
                    // o x x 
                    // x o o 
                    // x x x 
                    if (Matrix.instance.CheckMatchWithStateNone(i + 1, j - 2, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 2))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i + 1, j - 2));
                    }
                    // x x x
                    // x o o
                    // o x x
                    if (Matrix.instance.CheckMatchWithStateNone(i - 1, j - 2, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 2))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i - 1, j - 2));
                    }
                    // o x o o
                    if (Matrix.instance.CheckMatchWithStateNone(i, j - 3, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 2))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 2), new Vector2Int(i, j - 3));
                    }
                }
            }
        }
        // Vertical
        for (int j = 0; j < Matrix.instance.Column; j++)
        {
            for (int i = 1; i < Matrix.instance.Row; i++)
            {
                if (Matrix.instance.CheckMatch(i - 1, j, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 1, j) && Matrix.instance.CheckSelectableObject(i, j))
                {
                    // x x o
                    // x o x
                    // x o x
                    if (Matrix.instance.CheckMatchWithStateNone(i + 1, j + 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i + 1, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 1, j + 1));
                    }
                    // o x x
                    // x o x
                    // x o x
                    if (Matrix.instance.CheckMatchWithStateNone(i + 1, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i + 1, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 1, j - 1));
                    }
                    // o
                    // x
                    // o
                    // o
                    if (Matrix.instance.CheckMatchWithStateNone(i + 2, j, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i + 1, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i + 1, j), new Vector2Int(i + 2, j));
                    }
                    // x o x
                    // x o x
                    // x x o
                    if (Matrix.instance.CheckMatchWithStateNone(i - 2, j + 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 2, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 2, j), new Vector2Int(i - 2, j + 1));
                    }
                    // x o x
                    // x o x
                    // o x x
                    if (Matrix.instance.CheckMatchWithStateNone(i - 2, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 2, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 2, j), new Vector2Int(i - 2, j - 1));
                    }
                    // o
                    // o
                    // x
                    // o
                    if (Matrix.instance.CheckMatchWithStateNone(i - 3, j, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 2, j))
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
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            for (int j = 2; j < Matrix.instance.Column; j++)
            {
                if (Matrix.instance.CheckMatch(i, j - 2, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 2) && Matrix.instance.CheckSelectableObject(i, j))
                {
                    // x o x
                    // o x o
                    // x x x
                    if (Matrix.instance.CheckMatchWithStateNone(i + 1, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 1))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 1), new Vector2Int(i + 1, j - 1));
                    }
                    // x x x
                    // o x o
                    // x o x
                    if (Matrix.instance.CheckMatchWithStateNone(i - 1, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i, j - 1))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i, j - 1), new Vector2Int(i - 1, j - 1));
                    }
                }
            }
        }
        // Vertical
        for (int j = 0; j < Matrix.instance.Column; j++)
        {
            for (int i = 2; i < Matrix.instance.Row; i++)
            {
                if (Matrix.instance.CheckMatch(i - 2, j, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 2, j) && Matrix.instance.CheckSelectableObject(i, j))
                {
                    // x o x
                    // o x x
                    // x o x
                    if (Matrix.instance.CheckMatchWithStateNone(i - 1, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 1, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 1, j), new Vector2Int(i - 1, j - 1));
                    }
                    // x o x
                    // x x o
                    // x o x
                    if (Matrix.instance.CheckMatchWithStateNone(i - 1, j + 1, matrix[i, j].Properties.type) && Matrix.instance.CheckSelectableObject(i - 1, j))
                    {
                        return new KeyValuePair<Vector2Int, Vector2Int>(new Vector2Int(i - 1, j), new Vector2Int(i - 1, j + 1));
                    }
                }
            }
        }

        return null;
    }

    public void ObjectClicked(Object objectClicked)
    {
        //Debug.Log(Matrix.instance.m_ObjectsState[objectClicked.m_MatrixIndex.x, objectClicked.m_MatrixIndex.y]);

        // Hammer
        if (isHammering
            && !Matrix.instance.IsBusy()
            && !Matrix.instance.CheckFallingObjects()
            && !GameManager.instance.m_IsEndGame)
        {
            isHammering = false;
            PlayScene.instance.m_HammerHint.SetBool("isHint", false);
            int i = objectClicked.m_MatrixIndex.x, j = objectClicked.m_MatrixIndex.y;
            UseHammerEffect(i, j);

            Object[,] matrix = Matrix.instance.LockMatrix();
            CheckMatching(matrix);
            StartCoroutine(StartMatchCombo(matrix));
            Matrix.instance.FreeMatrix();

            return;
        }

        // Cant select when matrix is updating or end game or isFalling or out of turns
        if (!Matrix.instance.CheckSelectableObject(objectClicked.m_MatrixIndex.x, objectClicked.m_MatrixIndex.y)
            || Matrix.instance.IsBusy()
            || Matrix.instance.CheckFallingObjects()
            || GameManager.instance.m_IsEndGame
            || !GameManager.instance.CheckTurns())
            return;

        // Unselect
        if (firstSelected == objectClicked)
        {
            Unselect();
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

    // return false when matrix is busy
    public bool CheckMatching(Object[,] matrix, Vector2Int? first = null, Vector2Int? second = null)
    {
        bool isMatch = false;

        // Horizontal
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            int[] checkList = new int[Matrix.instance.Column];
            checkList[0] = 1;

            for (int j = 1; j < Matrix.instance.Column; j++)
            {
                if (Matrix.instance.CheckMatch(i, j - 1, matrix[i, j].Properties.type) && Matrix.instance.CheckMatchable(i, j))
                    checkList[j] = checkList[j - 1] + 1;
                else
                    checkList[j] = 1;
            }
            int count = 0;

            for (int j = Matrix.instance.Column - 1; j > -1; j--)
            {
                if (count > 0)
                {
                    Matrix.instance.SafeDestroyObject(i, j);
                    count--;
                }
                else if (checkList[j] >= 3 && matrix[i, j].Properties.isRare != true)
                {
                    if (checkList[j] >= 5)
                    {
                        if (first != null && second != null)
                        {
                            if (i == first.Value.x && j > first.Value.y && j - 5 < first.Value.y)
                                Matrix.instance.SafeToRainbow(first.Value.x, first.Value.y);
                            else if (i == second.Value.x && j > second.Value.y && j - 5 < second.Value.y)
                                Matrix.instance.SafeToRainbow(second.Value.x, second.Value.y);
                            else
                                Matrix.instance.SafeToRainbow(i, j);
                        }
                        else
                            Matrix.instance.SafeToRainbow(i, j);
                    }
                    else if (checkList[j] >= 4)
                    {
                        if (first != null && second != null)
                        {
                            if (i == first.Value.x && j > first.Value.y && j - 4 < first.Value.y)
                                Matrix.instance.SafeToLightning(first.Value.x, first.Value.y);
                            else if (i == second.Value.x && j > second.Value.y && j - 4 < second.Value.y)
                                Matrix.instance.SafeToLightning(second.Value.x, second.Value.y);
                            else
                                Matrix.instance.SafeToLightning(i, j);
                        }
                        else
                            Matrix.instance.SafeToLightning(i, j);
                    }
                    

                    Matrix.instance.SafeDestroyObject(i, j);
                    count = checkList[j] - 1;
                }
            }
        }

        // Vertical
        for (int j = 0; j < Matrix.instance.Column; j++)
        {
            int[] checkList = new int[Matrix.instance.Column];
            checkList[0] = 1;

            for (int i = 1; i < Matrix.instance.Row; i++)
            {
                if (Matrix.instance.CheckMatch(i - 1, j, matrix[i, j].Properties.type) && Matrix.instance.CheckMatchable(i, j))
                    checkList[i] = checkList[i - 1] + 1;
                else
                    checkList[i] = 1;
            }

            int count = 0;
            for (int i = Matrix.instance.Row - 1; i > -1; i--)
            {
                if (count > 0)
                {
                    if (Matrix.instance.CheckDestroyObject(i, j) && matrix[i, j].Properties.isRare != true)
                        Matrix.instance.SafeToBomb(i, j);
                    else
                        Matrix.instance.SafeDestroyObject(i, j);
                    count--;
                }
                else if (checkList[i] >= 3 && matrix[i, j].Properties.isRare != true)
                {
                    if (checkList[i] >= 5)
                    {
                        if (first != null && second != null)
                        {
                            if (j == first.Value.y && i > first.Value.x && i - 5 < first.Value.x)
                                Matrix.instance.SafeToRainbow(first.Value.x, first.Value.y);
                            else if (j == second.Value.y && i > second.Value.x && i - 5 < second.Value.x)
                                Matrix.instance.SafeToRainbow(second.Value.x, second.Value.y);
                            else
                                Matrix.instance.SafeToRainbow(i, j);
                        }
                        else
                            Matrix.instance.SafeToRainbow(i, j);
                    }
                    else if (checkList[i] >= 4)
                    {
                        if (first != null && second != null)
                        {
                            if (j == first.Value.y && i > first.Value.x && i - 4 < first.Value.x)
                                Matrix.instance.SafeToLightning(first.Value.x, first.Value.y);
                            else if (j == second.Value.y && i > second.Value.x && i - 4 < second.Value.x)
                                Matrix.instance.SafeToLightning(second.Value.x, second.Value.y);
                            else
                                Matrix.instance.SafeToLightning(i, j);
                        }
                        else
                            Matrix.instance.SafeToLightning(i, j);
                    }
                    else if (Matrix.instance.CheckDestroyObject(i, j) && matrix[i, j].Properties.isRare != true)
                        Matrix.instance.SafeToBomb(i, j);

                    Matrix.instance.SafeDestroyObject(i, j);
                    count = checkList[i] - 1;
                }
            }
        }

        for (int i = 0; i < Matrix.instance.Row; i++)
            for (int j = 0; j < Matrix.instance.Column; j++)
                if (Matrix.instance.CheckDestroyObject(i, j))
                {
                    isMatch = true;
                    break;
                }

        return isMatch;
    }

    // LOCK MATRIX FUNC
    public IEnumerator CheckSwap(Vector2Int first, Vector2Int second)
    {
        Object[,] matrix = null;
        while (matrix == null)
        {
            matrix = Matrix.instance.LockMatrix(); // Lock Matrix
            yield return null;
        }

        // Check if is swapable
        if (first.x == second.x && Mathf.Abs(first.y - second.y) == 1 ||
            first.y == second.y && Mathf.Abs(first.x - second.x) == 1)
        {
            yield return Matrix.instance.Swap(matrix[first.x, first.y], matrix[second.x, second.y]);

            if (CheckMatching(matrix, first, second) || matrix[first.x, first.y].Properties.isRare || matrix[second.x, second.y].Properties.isRare)
            {
                // start rare effect
                if (matrix[first.x, first.y].Properties.isRare)
                    UseRareObject(matrix[first.x, first.y], matrix[second.x, second.y]);
                if (matrix[second.x, second.y].Properties.isRare)
                    UseRareObject(matrix[second.x, second.y], matrix[first.x, first.y]);

                // start match chain
                yield return StartMatchCombo(matrix);

                // minus 1 turn
                GameManager.instance.DecreaseTurn();
            }
            else
            {
                AudioManager.instance.PlaySFX(AudioManager.SFX_WRONG_MOVE);
                StartCoroutine(matrix[first.x, first.y].SetWarn());
                yield return matrix[second.x, second.y].SetWarn();

                yield return Matrix.instance.Swap(matrix[first.x, first.y], matrix[second.x, second.y]);
            }
        }

        Matrix.instance.FreeMatrix(); // Free Matrix
    }

    public IEnumerator StartMatchCombo(Object[,] matrix)
    {
        Object[,] currMatrix = matrix;
        bool isLock  = false;
        if (!Matrix.instance.IsBusy())
        {
            isLock = true;
            currMatrix = Matrix.instance.LockMatrix();
        }

        int combo = 0;

        // first loop here (avoid duplicate checkmatching)
        while (Matrix.instance.CheckFallingObjects())
            yield return null;
        {
            combo++;
            GameManager.instance.UpdateScores(UpdateMatrix(currMatrix), combo);
            yield return UpdateSliding(currMatrix);
        }

        while (Matrix.instance.CheckFallingObjects())
            yield return null;
        while (CheckMatching(currMatrix))
        {
            while (Matrix.instance.CheckFallingObjects())
                yield return null;

            combo++;
            GameManager.instance.UpdateScores(UpdateMatrix(currMatrix), combo);
            yield return UpdateSliding(currMatrix);

            while (Matrix.instance.CheckFallingObjects())
                yield return null;
        }

        while (Matrix.instance.CheckFallingObjects())
            yield return null;

        if (CheckCanContinue(currMatrix) == null)
        {
            StartCoroutine(GameManager.instance.OutOfMove());
        }

        if (isLock)
            Matrix.instance.FreeMatrix();
    }

    private int UpdateMatrix(Object[,] matrix)
    {
        int matchedObjectcount = 0;

        // update mission
        // TODO: consider using Observer pattern
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            for (int j = 0; j < Matrix.instance.Column; j++)
            {
                if (Matrix.instance.CheckObjectForPoints(i ,j))
                {
                    GameManager.instance.UpdateClearMission(matrix[i, j].Properties);
                    matchedObjectcount++;
                }
            }
        }

        // Use effect if rare
        ChainRareObject(matrix);

        // Transform to rare object
        Matrix.instance.TransfromObject();

        // Update Falling Column
        int[] columnQueue = new int[Matrix.instance.Column];
        for (int i = 0; i < Matrix.instance.Column; i++)
            columnQueue[i] = 0;
        for (int j = 0; j < Matrix.instance.Column; j++)
        {
            UpdateFallingColumn(matrix, j, ref columnQueue[j]);
        }

        // Reset objects state
        //Matrix.instance.InitObjectsState(false);

        return matchedObjectcount;
    }

    private IEnumerator UpdateSliding(Object[,] matrix)
    {
        while (Matrix.instance.CheckFallingObjects())
            yield return null;
        int[] columnQueue = new int[Matrix.instance.Column];
        for (int i = 0; i < Matrix.instance.Column; i++)
            columnQueue[i] = 0;
        bool canContinueSliding = true;
        while (canContinueSliding)
        {
            canContinueSliding = false;
            bool skip = false;
            for (int i = 1; i < Matrix.instance.Row; i++)
            {
                if (skip)
                    break;
                for (int j = 0; j < Matrix.instance.Column; j++)
                {
                    if (Matrix.instance.CheckIfObjectCanFall(i, j) && !Matrix.instance.CheckDestroyObject(i, j) && !Matrix.instance.CheckEmptyObject(i, j)
                        && !Matrix.instance.CheckDestroyObject(i - 1, j) && !Matrix.instance.CheckEmptyObject(i - 1, j))
                    {
                        if (Matrix.instance.CheckEmptyObject(i - 1, j - 1))
                        {
                            canContinueSliding = true;
                            matrix[i - 1, j - 1].SetObjectProperties(matrix[i, j].Properties);
                            matrix[i - 1, j - 1].InheritDestinies(matrix[i, j].m_Destinies);
                            matrix[i - 1, j - 1].m_Destinies.Enqueue(matrix[i - 1, j - 1].m_MatrixPosition);
                            matrix[i - 1, j - 1].Position = matrix[i, j].transform.position;
                            Matrix.instance.ResetObjectState(i - 1, j - 1);
                            Matrix.instance.SetEmptyObject(i, j);

                            UpdateFallingColumn(matrix, j - 1, ref columnQueue[j - 1]);
                            UpdateFallingColumn(matrix, j, ref columnQueue[j]);
                            skip = true;
                        }
                        else if (Matrix.instance.CheckEmptyObject(i - 1, j + 1))
                        {
                            canContinueSliding = true;
                            matrix[i - 1, j + 1].SetObjectProperties(matrix[i, j].Properties);
                            matrix[i - 1, j + 1].InheritDestinies(matrix[i, j].m_Destinies);
                            matrix[i - 1, j + 1].m_Destinies.Enqueue(matrix[i - 1, j + 1].m_MatrixPosition);
                            matrix[i - 1, j + 1].Position = matrix[i, j].transform.position;
                            Matrix.instance.ResetObjectState(i - 1, j + 1);
                            Matrix.instance.SetEmptyObject(i, j);

                            UpdateFallingColumn(matrix, j + 1, ref columnQueue[j + 1]);
                            UpdateFallingColumn(matrix, j, ref columnQueue[j]);
                            skip = true;
                        } 
                    }
                }
            }  
        }
    }

    private void UpdateFallingColumn(Object[,] matrix, int j, ref int columnQueue)
    {
        for (int i = 0; i < Matrix.instance.Row; i++)
        {
            if (Matrix.instance.CheckDestroyObject(i, j) || Matrix.instance.CheckEmptyObject(i, j))
            {
                bool isBlocked = false;
                int r = i + 1;
                while (r < Matrix.instance.Row)
                {
                    if (!Matrix.instance.CheckIfObjectCanFall(r, j))
                    {
                        isBlocked = true;
                        break;
                    }

                    if (!Matrix.instance.CheckDestroyObject(r, j) && !Matrix.instance.CheckEmptyObject(r, j))
                    {
                        matrix[i, j].SetObjectProperties(matrix[r, j].Properties);
                        matrix[i, j].m_Destinies.Enqueue(matrix[i, j].m_MatrixPosition);
                        matrix[i, j].Position = matrix[r, j].transform.position;
                        Matrix.instance.ResetObjectState(i, j);
                        Matrix.instance.SetEmptyObject(r, j);
                        break;
                    }
                    r++;
                }
                if (Matrix.instance.CheckDestroyObject(i, j) || Matrix.instance.CheckEmptyObject(i, j))
                {
                    if (!isBlocked)
                    {
                        matrix[i, j].SetObjectProperties(DataManager.instance.GetRandomObject());
                        matrix[i, j].m_Destinies.Enqueue(matrix[i, j].m_MatrixPosition);
                        Vector3 pos = Matrix.instance.transform.position;
                        pos.x += j * Matrix.instance.ObjectSize.x - Matrix.instance.ObjectSize.x * (Matrix.instance.Column - 1) / 2;
                        pos.y += (Matrix.instance.Row + columnQueue + 1) * Matrix.instance.ObjectSize.y - Matrix.instance.ObjectSize.y * (Matrix.instance.Row - 1) / 2;
                        matrix[i, j].Position = pos;
                        Matrix.instance.ResetObjectState(i, j);
                        columnQueue++;
                    }
                    else
                    {
                        Matrix.instance.SetEmptyObject(i, j);
                    }
                }
            }
        }    
    }

    public void UseRareObject(Object rareObj, Object affectedObj)
    {
        Matrix.instance.SafeDestroyObject(rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y);
        Matrix.instance.m_EffectQueue.Dequeue();
        switch (rareObj.Properties.type)
        {
            case IngameObject.ObjectType.Rainbow:
                {
                    UseRainbowEffect(rareObj, affectedObj.Properties.type);
                    break;
                }
            case IngameObject.ObjectType.Lightning:
                {
                    UseLightningEffect(rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y);
                    break;
                }
            case IngameObject.ObjectType.Bomb:
                {
                    UseBombEffect(rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y);
                    break;
                }
            case IngameObject.ObjectType.Clock:
                {
                    UseClockEffect(rareObj.m_MatrixIndex.x, rareObj.m_MatrixIndex.y);
                    break;
                }
        }
    }

    public void ChainRareObject(Object[,] matrix)
    {
        while (Matrix.instance.m_EffectQueue.Count > 0)
        {
            Object rareObject = Matrix.instance.m_EffectQueue.Dequeue();
            switch (rareObject.Properties.type)
            {
                case IngameObject.ObjectType.Rainbow:
                    {
                        UseRainbowEffect(rareObject, DataManager.instance.GetRandomCommonObject().type);
                        break;
                    }
                case IngameObject.ObjectType.Lightning:
                    {
                        UseLightningEffect(rareObject.m_MatrixIndex.x, rareObject.m_MatrixIndex.y);
                        break;
                    }
                case IngameObject.ObjectType.Bomb:
                    {
                        UseBombEffect(rareObject.m_MatrixIndex.x, rareObject.m_MatrixIndex.y);
                        break;
                    }
                case IngameObject.ObjectType.Clock:
                    {
                        UseClockEffect(rareObject.m_MatrixIndex.x, rareObject.m_MatrixIndex.y);
                        break;
                    }
            }
                
        }
    }

    public void UseRainbowEffect(Object rainbow, IngameObject.ObjectType type)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_RAINBOW);
        Object[,] matrix = Matrix.instance.GetMatrix();

        if (type != IngameObject.ObjectType.Rainbow)
        {
            for (int i = 0; i < Matrix.instance.Row; i++)
                for (int j = 0; j < Matrix.instance.Column; j++)
                {
                    if (matrix[i, j].Properties.type == type)
                    {
                        Matrix.instance.SafeRainbowVFX(rainbow.m_MatrixIndex.x, rainbow.m_MatrixIndex.y, i, j);
                        Matrix.instance.SafeDestroyObject(i, j);
                    }
                }
        }
        else
        {
            for (int i = 0; i < Matrix.instance.Row; i++)
                for (int j = 0; j < Matrix.instance.Column; j++)
                {
                    Matrix.instance.SafeRainbowVFX(rainbow.m_MatrixIndex.x, rainbow.m_MatrixIndex.y, i, j);
                    Matrix.instance.SafeDestroyObject(i, j);
                }
        }
    }

    public void UseBombEffect(int i, int j)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_BOMB);
        Matrix.instance.SafeBombVFX(i, j);
        Matrix.instance.SafeDestroyObject(i + 1, j - 1); Matrix.instance.SafeDestroyObject(i + 1, j); Matrix.instance.SafeDestroyObject(i + 1, j + 1);
        Matrix.instance.SafeDestroyObject(i, j - 1); /*Matrix.instance.SafeDestroyObject(i, j);*/ Matrix.instance.SafeDestroyObject(i, j + 1);
        Matrix.instance.SafeDestroyObject(i - 1, j - 1); Matrix.instance.SafeDestroyObject(i - 1, j); Matrix.instance.SafeDestroyObject(i - 1, j + 1);
    }

    public void UseLightningEffect(int i, int j)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_LIGHTNING);
        Matrix.instance.SafeLightningVFX(i, j);
        for (int r = 0; r < Matrix.instance.Row; r++)
        {
            Matrix.instance.SafeDestroyObject(r, j);
        }
        for (int c = 0; c < Matrix.instance.Column; c++)
        {
            Matrix.instance.SafeDestroyObject(i, c);
        }
    }

    public void UseClockEffect(int i, int j)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_CLOCK);
        Matrix.instance.SafeClockVFX(i, j, "+10s");
        GameManager.instance.AddTime(10f);
    }

    public void UseHammerEffect(int i, int j)
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_HAMMER);
        Matrix.instance.SafeHammerVFX(i, j);
        Matrix.instance.SafeCompleteDestroyObject(i + 1, j - 1); Matrix.instance.SafeCompleteDestroyObject(i + 1, j); Matrix.instance.SafeCompleteDestroyObject(i + 1, j + 1);
        Matrix.instance.SafeCompleteDestroyObject(i, j - 1); Matrix.instance.SafeCompleteDestroyObject(i, j); Matrix.instance.SafeCompleteDestroyObject(i, j + 1);
        Matrix.instance.SafeCompleteDestroyObject(i - 1, j - 1); Matrix.instance.SafeCompleteDestroyObject(i - 1, j); Matrix.instance.SafeCompleteDestroyObject(i - 1, j + 1);
    }

    public void SetHintPair(KeyValuePair<Vector2Int, Vector2Int>? pair)
    {
        currHintedPair = pair;
    }

    public void SetHint(bool isHint)
    {
        Object[,] matrix = Matrix.instance.GetMatrix();

        if (currHintedPair == null || matrix == null)
            return;
        else
        {
            matrix[currHintedPair.Value.Key.x, currHintedPair.Value.Key.y].SetHint(isHint);
            matrix[currHintedPair.Value.Value.x, currHintedPair.Value.Value.y].SetHint(isHint);
        }
    }
}
