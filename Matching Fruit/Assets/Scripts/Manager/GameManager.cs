using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // TODO: Create seperate class for mission management & shopping
    private int m_Score = 0;
    private float m_Time;
    private int m_Turns;
    private int m_Balance;
    private Mission m_CurrMission;

    internal bool m_IsEndGame = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DataManager.instance.SetLevelData(ref m_Score, ref m_Time, ref m_Turns, ref m_Balance);

        m_CurrMission = DataManager.instance.GetRandomClearMission();
        PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
        PlayScene.instance.m_Mission.SetSprite((m_CurrMission as ClearMission).ingameObject.sprite);

        StartCoroutine(Matrix.instance.ResetMatrix());
        m_IsEndGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsEndGame)
            return;

        StartCoroutine(UpdateTime());

        if (!CheckTurns())
        {
            StartCoroutine(OutOfTurn());
        }

        if (m_CurrMission.counter <= 0)
        {
            m_Balance += m_CurrMission.price;
            PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

            m_CurrMission = DataManager.instance.GetRandomClearMission();
            PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
            PlayScene.instance.m_Mission.SetSprite((m_CurrMission as ClearMission).ingameObject.sprite);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(Define.SCENE_PLAY);
    }

    public void AddTime(float value)
    {
        m_Time += value;
    }

    public void UpdateScores(int matchedObjectCount, int combo)
    {
        if (matchedObjectCount <= 0)
            return;

        int count = 3;
        int score = 0;
        while (count > 0)
        {
            score += 50;
            count--;
        }
        if (matchedObjectCount > 3)
            score += (matchedObjectCount - 3) * 100;

        m_Score += score * combo;
        PlayScene.instance.m_Score.SetText(m_Score.ToString());
        DataManager.instance.CheckGoal(m_Score);

        if (combo > 1)
            StartCoroutine(PlayScene.instance.m_Combo.SetFadingText("x" + combo.ToString()));
    }

    public void DecreaseTurn()
    {
        m_Turns--;
        PlayScene.instance.m_Turns.SetNumber(m_Turns);
    }

    public void AddTurns(int value)
    {
        m_Turns += value;
        PlayScene.instance.m_Turns.SetNumber(m_Turns);
    }
    
    public bool CheckTurns()
    {
        return m_Turns > 0;
    }

    public void UpdateClearMission(IngameObject ingameObject)
    {
        if (m_CurrMission is ClearMission)
        {
            ClearMission mission = m_CurrMission as ClearMission;
            if (mission.ingameObject.type == ingameObject.type)
            {
                m_CurrMission.counter--;
                PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
            }      
        }
    }

    public IEnumerator ForceEndGame()
    {
        while (Matrix.instance.IsBusy() || Matrix.instance.CheckFallingObjects())
        {
            yield return null;
        }
        m_IsEndGame = true;
        AudioManager.instance.StopBGM(0.5f);
        PlayScene.instance.Result(m_Score, "EHHH ?!!!!");
    }

    private IEnumerator UpdateTime()
    {
        m_Time -= Time.deltaTime;
        if (m_Time < 0f)
        {
            m_Time = 0f;
            m_IsEndGame = true;
            while (Matrix.instance.IsBusy() || Matrix.instance.CheckFallingObjects())
            {
                yield return null;
            }
            AudioManager.instance.StopBGM(1.5f);
            AudioManager.instance.PlaySFX(AudioManager.SFX_ENDGAME);
            yield return new WaitForSeconds(1.5f);
            PlayScene.instance.Result(m_Score, "OUT OF TIME !!!");
        }

        PlayScene.instance.m_Time.SetNumber((int)Mathf.Floor(m_Time));
    }

    public IEnumerator OutOfMove()
    {
        while (Matrix.instance.IsBusy() || Matrix.instance.CheckFallingObjects())
        {
            yield return null;
        }
        if (m_IsEndGame)
            yield break;

        Matrix.instance.LockMatrix();

        yield return PlayScene.instance.m_Message.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        yield return PlayScene.instance.m_Message.SetActive(false);

        yield return Matrix.instance.ResetMatrix();

        Matrix.instance.FreeMatrix();
    }

    public IEnumerator OutOfTurn()
    {
        if (m_Balance >= 5)
        {
            PlayScene.instance.m_AddTurnHint.SetBool("isHint", true);
            yield break;
        }
        while (Matrix.instance.IsBusy() || Matrix.instance.CheckFallingObjects())
        {
            yield return null;
        }
        AudioManager.instance.StopBGM(1.5f);
        AudioManager.instance.PlaySFX(AudioManager.SFX_ENDGAME);
        yield return new WaitForSeconds(1.5f);
        m_IsEndGame = true;

        PlayScene.instance.Result(m_Score, "OUT OF TURNS !!!");
    }

    // Currently using static items (hard code)
    public void BuyHammer()
    {
        if (m_Balance < 30 || Matrix.instance.IsBusy() || Mechanic.instance.IsHammering)
            return;

        m_Balance -= 30;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        PlayScene.instance.m_HammerHint.SetBool("isHint", true);
        Mechanic.instance.Unselect();
        Mechanic.instance.BuyHammer();
    }

    public void BuyAddTurn()
    {
        if (m_Balance < 10 || Matrix.instance.IsBusy())
            return;

        m_Balance -= 10;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        PlayScene.instance.m_AddTurnHint.SetBool("isHint", false);
        AddTurns(10);
    }

    public void BuyHint()
    {
        if (m_Balance < 2 || Matrix.instance.IsBusy())
            return;

        m_Balance -= 2;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        Mechanic.instance.SetHintPair(Mechanic.instance.CheckCanContinue(Matrix.instance.LockMatrix())); // Lock Matrix
        Mechanic.instance.SetHint(true);
        Matrix.instance.FreeMatrix(); // Free Matrix
    }

}
