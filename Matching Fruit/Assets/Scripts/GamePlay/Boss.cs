using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private static string BOSS_MOVE_PARAM = "isMove";
    private static string BOSS_CHARGE_PARAM = "isCharge";

    [SerializeField] private Animator m_Animator;
    [SerializeField] private AnimationCurve m_MoveCurve;
    [SerializeField] private float m_CoolDown = 30f;
    //[SerializeField] private int m_SkillsPerPos = 2;

    private Vector2Int m_MatrixIndex = Vector2Int.zero;
    private float m_CurrCooldown;
    private int m_CurrSkillsPerPos;
    private bool m_isCharging = false;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrCooldown = m_CoolDown;
        m_CurrSkillsPerPos = 2;
        StartCoroutine(StartNewPos());
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isCharging && !GameManager.instance.m_IsEndGame)
            m_CurrCooldown -= Time.deltaTime;

        if (m_CurrCooldown < Mathf.Epsilon)
        {
            m_CurrCooldown = m_CoolDown;

            if (m_CurrSkillsPerPos > 0)
            {
                switch (m_CurrSkillsPerPos)
                {
                    default:
                    case 2:
                        {
                            StartCoroutine(SpawnIce());
                            break;
                        }
                    case 1:
                        {
                            StartCoroutine(SpawnChain());
                            break;
                        }
                }
            }
            else 
            {
                //StartCoroutine(FreeBlock(m_MatrixIndex.x, m_MatrixIndex.y));
                StartCoroutine(StartNewPos());
            }
        }
    }

    private IEnumerator StartNewPos()
    {
        Vector2Int oldPos = m_MatrixIndex;
        while (m_MatrixIndex == oldPos)
            m_MatrixIndex = new Vector2Int(Random.Range(0, Matrix.instance.Row - 1), Random.Range(0, Matrix.instance.Column - 1));

        Vector3 pos = Matrix.instance.transform.position;
        pos.x += m_MatrixIndex.y * Matrix.instance.ObjectSize.x - Matrix.instance.ObjectSize.x * (Matrix.instance.Column - 1) / 2;
        pos.y += m_MatrixIndex.x * Matrix.instance.ObjectSize.y - Matrix.instance.ObjectSize.y * (Matrix.instance.Row - 1) / 2;
        yield return MoveTo(pos, 1.0f);

        yield return SpawnBlock();
    }

    private IEnumerator MoveTo(Vector3 destiny, float time)
    {
        m_Animator.SetBool(BOSS_MOVE_PARAM, true);

        float currTime = time;
        Vector3 originalPos = transform.position;
        while (currTime > 0)
        {
            transform.position = Vector3.Lerp(destiny, originalPos, m_MoveCurve.Evaluate(currTime / time));
            currTime -= Time.deltaTime;
            yield return null;
        }
        transform.position = Vector3.Lerp(destiny, originalPos, 0);

        m_Animator.SetBool(BOSS_MOVE_PARAM, false);
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

        Mechanic.instance.CheckMatching(matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(matrix));

        Matrix.instance.FreeMatrix(); // Free Matrix
    }

    private IEnumerator SpawnBlock()
    {
        m_isCharging = true;
        m_Animator.SetBool(BOSS_CHARGE_PARAM, true);
        yield return new WaitForSeconds(2.0f);

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

        Mechanic.instance.CheckMatching(matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(matrix));

        Matrix.instance.FreeMatrix(); // Free Matrix

        m_Animator.SetBool(BOSS_CHARGE_PARAM, false);
        m_isCharging = false;
    }

    private IEnumerator SpawnChain()
    {
        m_isCharging = true;
        m_Animator.SetBool(BOSS_CHARGE_PARAM, true);
        yield return new WaitForSeconds(2.0f);

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
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x + 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y - 2);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y + 1);
        Matrix.instance.SetWarnSafe(m_MatrixIndex.x, m_MatrixIndex.y + 2);

        Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y - 2);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        //Matrix.instance.SetWarnSafe(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);
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
        //Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y - 1);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x + 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y - 2);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y - 2);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y - 1);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y);
        //Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 1, m_MatrixIndex.y + 2);

        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y - 2);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y - 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y + 1);
        Matrix.instance.SetStateChained(m_MatrixIndex.x - 2, m_MatrixIndex.y + 2);

        Mechanic.instance.CheckMatching(matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(matrix));

        Matrix.instance.FreeMatrix(); // Free Matrix

        m_Animator.SetBool(BOSS_CHARGE_PARAM, false);
        m_isCharging = false;
    }

    private IEnumerator SpawnIce()
    {
        m_isCharging = true;
        m_Animator.SetBool(BOSS_CHARGE_PARAM, true);
        yield return new WaitForSeconds(2.0f);

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

        Mechanic.instance.CheckMatching(matrix);
        StartCoroutine(Mechanic.instance.StartMatchCombo(matrix));

        Matrix.instance.FreeMatrix(); // Free Matrix

        m_Animator.SetBool(BOSS_CHARGE_PARAM, false);
        m_isCharging = false;
    }
}
