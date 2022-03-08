using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float m_CoolDown = 30f;
    //[SerializeField] private int m_SkillsPerPos = 2;
    private Vector2Int m_MatrixIndex = Vector2Int.zero;
    private float m_CurrCooldown = 10f;
    private int m_CurrSkillsPerPos = 2;
    //private int m_CurrRing = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_MatrixIndex = new Vector2Int(Random.Range(0, Matrix.instance.Row - 1), Random.Range(0, Matrix.instance.Column - 1));
        StartCoroutine(SpawnBlock());
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrCooldown -= Time.deltaTime;
        if (m_CurrCooldown < 0)
        {
            m_CurrCooldown = m_CoolDown;

            if (m_CurrSkillsPerPos == 2)
                StartCoroutine(SpawnIce());
            if (m_CurrSkillsPerPos == 1)
                StartCoroutine(SpawnChain());
            if (m_CurrSkillsPerPos == 0)
            {
                //StartCoroutine(FreeBlock(m_MatrixIndex.x, m_MatrixIndex.y));
                m_MatrixIndex = new Vector2Int(Random.Range(0, Matrix.instance.Row - 1), Random.Range(0, Matrix.instance.Column - 1));
                StartCoroutine(SpawnBlock());
            }  
        }
    }

    private IEnumerator FreeBlock(int i, int j)
    {
        Object[,] matrix = null;
        while (matrix == null)
        {
            matrix = Matrix.instance.LockMatrix(); // Lock Matrix
            yield return null;
        }
        while (Matrix.instance.CheckFallingObjects())
            yield return null;

        Mechanic.instance.Unselect();
        m_CurrSkillsPerPos = 2;

        //free block
        Matrix.instance.FreeStateBlock(i + 1, j - 1);
        Matrix.instance.FreeStateBlock(i + 1, j);
        Matrix.instance.FreeStateBlock(i + 1, j + 1);

        Matrix.instance.FreeStateBlock(i, j - 1);
        Matrix.instance.FreeStateBlock(i, j);
        Matrix.instance.FreeStateBlock(i, j + 1);

        Matrix.instance.FreeStateBlock(i - 1, j - 1);
        Matrix.instance.FreeStateBlock(i - 1, j);
        Matrix.instance.FreeStateBlock(i - 1, j + 1);

        StartCoroutine(Mechanic.instance.StartMatchCombo(matrix));

        Matrix.instance.FreeMatrix(); // Free Matrix
    }

    private IEnumerator SpawnBlock()
    {
        Object[,] matrix = null;
        while (matrix == null)
        {
            matrix = Matrix.instance.LockMatrix(); // Lock Matrix
            yield return null;
        }
        while (Matrix.instance.CheckFallingObjects())
            yield return null;

        Mechanic.instance.Unselect();
        m_CurrSkillsPerPos = 2;

        // Set warning
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y + 1);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);
        yield return matrix[m_MatrixIndex.x, m_MatrixIndex.y].SetWarn();

        //Set block
        Matrix.instance.SetStateBlock(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);

        Matrix.instance.SetStateBlock(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x, m_MatrixIndex.y);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x, m_MatrixIndex.y + 1);

        Matrix.instance.SetStateBlock(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        Matrix.instance.SetStateBlock(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);

        Matrix.instance.FreeMatrix(); // Free Matrix
    }

    private IEnumerator SpawnChain()
    {
        Object[,] matrix = null;
        while (matrix == null)
        {
            matrix = Matrix.instance.LockMatrix(); // Lock Matrix
            yield return null;
        }
        while (Matrix.instance.CheckFallingObjects())
            yield return null;

        Mechanic.instance.Unselect();
        m_CurrSkillsPerPos--;

        // Set warning
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 2, m_MatrixIndex.y - 2);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 2, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 2, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 2, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 2, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y - 2);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y - 2);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y - 2);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 2, m_MatrixIndex.y - 2);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 2, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 2, m_MatrixIndex.y);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 2, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 2, m_MatrixIndex.y + 2);
        yield return matrix[m_MatrixIndex.x, m_MatrixIndex.y].SetWarn();

        //Set ice
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 2, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 2, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 2, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 2, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 2, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y + 2);

        Matrix.instance.FreeMatrix(); // Free Matrix
    }

    private IEnumerator SpawnIce()
    {
        Object[,] matrix = null;
        while (matrix == null)
        {
            matrix = Matrix.instance.LockMatrix(); // Lock Matrix
            yield return null;
        }
        while (Matrix.instance.CheckFallingObjects())
            yield return null;

        Mechanic.instance.Unselect();
        m_CurrSkillsPerPos--;

        // Set warning
        for (int r = 0; r < Matrix.instance.Row; r++)
        {
            if (r != m_MatrixIndex.x)
                Matrix.instance.SetWarnSafe(r, m_MatrixIndex.y);
        }
        for (int c = 0; c < Matrix.instance.Row; c++)
        {
            if (c != m_MatrixIndex.y)
                Matrix.instance.SetWarnSafe(m_MatrixIndex.x, c);
        }
        yield return matrix[m_MatrixIndex.x, m_MatrixIndex.y].SetWarn();

        //Set chain
        for (int r = 0; r < Matrix.instance.Row; r++)
        {
            Matrix.instance.SetStateFreeze(r, m_MatrixIndex.y);
        }
        for (int c = 0; c < Matrix.instance.Row; c++)
        {
            Matrix.instance.SetStateFreeze(m_MatrixIndex.x, c);
        }

        Matrix.instance.FreeMatrix(); // Free Matrix
    }
}
