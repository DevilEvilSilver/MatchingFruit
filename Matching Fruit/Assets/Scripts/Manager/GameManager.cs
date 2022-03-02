using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private int m_Turns;
    public int m_Score = 0;

    // TODO: Create seperate class for mission management & shopping
    private int m_Balance = 0;
    private Mission m_CurrMission;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CurrMission = DataManager.instance.GetRandomClearMission();

        PlayScene.instance.m_Turns.SetText(m_Turns.ToString());
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());
        PlayScene.instance.m_MissionCounter.SetText(m_CurrMission.counter.ToString());
        PlayScene.instance.m_Mission.SetSprite((m_CurrMission as ClearMission).ingameObject.sprite);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Turns <= 0)
        {
            Debug.Log("GAME OVER !!!!!!");
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

    public void OutOfMove()
    {
        if (m_Balance >= 25)
        {
            PlayScene.instance.m_refreshHint.SetSelected(true);
            return;
        }
        Debug.Log("GAME OVER !!!!!!");
    }

    // Currently using static items (hard code)
    public void BuyRefresh()
    {
        if (m_Balance < 25)
            return;

        m_Balance -= 25;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        PlayScene.instance.m_refreshHint.SetSelected(false);
        Matrix.instance.ResetMatrix();
    }

    public void BuyAddTurn()
    {
        if (m_Balance < 5)
            return;

        m_Balance -= 5;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        AddTurns(1);
    }

    public void BuyHint()
    {
        if (m_Balance < 2)
            return;

        m_Balance -= 2;
        PlayScene.instance.m_Balance.SetText(m_Balance.ToString());

        Debug.Log("Ai biet <(\") ?");
    }

}
