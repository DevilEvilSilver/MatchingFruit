using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : MonoBehaviour
{
    public static PlayScene instance;

    public FadingText m_Combo;
    public TextUI m_Score;
    public TextUI m_Turns;
    public TextUI m_Time;
    public TextUI m_Balance;
    public TextUI m_MissionCounter;
    public ImageUI m_Mission;

    public HintEffect m_RefreshHint;
    public HintEffect m_AddTurnHint;

    public GameObject m_Info;
    public CanvasGroup m_ResultCanvasGroup;
    public TextUI m_Result;
    public TextUI m_Reason;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Retry()
    {
        //Resume();
        m_ResultCanvasGroup.alpha = 0f;
        m_ResultCanvasGroup.interactable = false;
        m_ResultCanvasGroup.blocksRaycasts = false;
        GameManager.instance.ResetGame();
    }

    public void Result(string result, string reason)
    {
        m_ResultCanvasGroup.alpha = 1f;
        m_ResultCanvasGroup.interactable = true;
        m_ResultCanvasGroup.blocksRaycasts = true;
        m_Result.SetText(result);
        m_Reason.SetText(reason);
        //Pause();
    }

    public void ToggleInfo()
    {
        m_Info.SetActive(!m_Info.activeSelf);
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
