using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScene : MonoBehaviour
{
    public static PlayScene instance;

    public TextUI m_Score;
    public FadingText m_Combo;
    public TextUI m_Turns;
    public TextUI m_Balance;
    public TextUI m_MissionCounter;
    public ImageUI m_Mission;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void BuyRefresh()
    {
        GameManager.instance.BuyRefresh();
    }

    public void BuyAddTurn()
    {
        GameManager.instance.BuyAddTurn();
    }

    public void BuyHint()
    {
        GameManager.instance.BuyHint();
    }

}
