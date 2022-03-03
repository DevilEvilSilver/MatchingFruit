using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int time = 180;
    [SerializeField] private int turns = 30;
    [SerializeField] private int balance = 0;

    // TODO: Create seperate class for mission management & shopping
    private float m_Time;
    private int m_Turns;
    private int m_Balance;
    private int m_Score = 0;
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
        m_CurrMission = DataManager.instance.GetRandomClearMission();
        m_Time = time;
        m_Turns = turns;
        m_Balance = balance;

        PlayScene.instance.m_Time.SetText(Mathf.Floor(m_Time).ToString());
        PlayScene.instance.m_Turns.SetText(m_Turns.ToString());
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());
        PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
        PlayScene.instance.m_Mission.SetSprite((m_CurrMission as ClearMission).ingameObject.sprite);
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
        m_Score = 0;
        m_Time = time;
        m_Turns = turns;
        m_Balance = balance;
        m_CurrMission = DataManager.instance.GetRandomClearMission();

        PlayScene.instance.m_Score.SetText(m_Score.ToString());
        PlayScene.instance.m_Time.SetText(Mathf.Floor(m_Time).ToString());
        PlayScene.instance.m_Turns.SetText(m_Turns.ToString());
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());
        PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
        PlayScene.instance.m_Mission.SetSprite((m_CurrMission as ClearMission).ingameObject.sprite);

        Matrix.instance.ResetMatrix();
        m_IsEndGame = false;
    }

    private IEnumerator UpdateTime()
    {
        m_Time -= Time.deltaTime;
        if (m_Time < 0f)
        {
            m_Time = 0f;
            m_IsEndGame = true;
            while (true)
            {
                if (Matrix.instance.CheckMatching())
                    yield return null;
                else
                {
                    yield return new WaitForSeconds(1.0f);
                    break;
                }    
            }
            PlayScene.instance.Result(m_Score.ToString(), "OUT OF TIME !!!");
        }

        PlayScene.instance.m_Time.SetText(Mathf.Floor(m_Time).ToString());
    }

    public void AddTime(float value)
    {
        m_Time += value;
    }

    public void UpdateScores(int matchedObjectCount, int combo)
    {
        m_Score += matchedObjectCount * combo * 50;
        PlayScene.instance.m_Score.SetText(m_Score.ToString());

        if (combo > 1)
            PlayScene.instance.m_Combo.SetText("x" + combo.ToString());
    }

    public void DecreaseTurn()
    {
        m_Turns--;
        PlayScene.instance.m_Turns.SetText(m_Turns.ToString());
    }

    public void AddTurns(int value)
    {
        m_Turns += value;
        PlayScene.instance.m_Turns.SetText(m_Turns.ToString());
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

    public IEnumerator OutOfMove()
    {
        if (m_Balance >= 25)
        {
            PlayScene.instance.m_RefreshHint.SetSelected(true);
            yield break;
        }
        yield return new WaitForSeconds(1.0f);
        m_IsEndGame = true;

        PlayScene.instance.Result(m_Score.ToString(), "OUT OF MOVES !!!");
    }

    public IEnumerator OutOfTurn()
    {
        if (m_Balance >= 5)
        {
            PlayScene.instance.m_AddTurnHint.SetSelected(true);
            yield break;
        }
        yield return new WaitForSeconds(1.0f);
        m_IsEndGame = true;

        PlayScene.instance.Result(m_Score.ToString(), "OUT OF TURNS !!!");
    }

    // Currently using static items (hard code)
    public void BuyRefresh()
    {
        if (m_Balance < 25)
            return;

        m_Balance -= 25;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        PlayScene.instance.m_RefreshHint.SetSelected(false);
        Matrix.instance.ResetMatrix();
    }

    public void BuyAddTurn()
    {
        if (m_Balance < 5)
            return;

        m_Balance -= 5;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        PlayScene.instance.m_AddTurnHint.SetSelected(false);
        AddTurns(5);
    }

    public void BuyHint()
    {
        if (m_Balance < 2)
            return;

        m_Balance -= 2;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        Matrix.instance.SetHintPair(Matrix.instance.CheckCanContinue());
        Matrix.instance.SetHint(true);
    }

}
