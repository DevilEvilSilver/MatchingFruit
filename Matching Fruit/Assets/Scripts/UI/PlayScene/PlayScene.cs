using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayScene : MonoBehaviour
{
    public static PlayScene instance;

    public FadingGroup m_Message;
    public FadingText m_Combo;
    public TextUI m_Score;
    public TextUI m_Turns;
    public TextUI m_Time;
    public TextUI m_Balance;
    public TextUI m_MissionCounter;
    public ImageUI m_Mission;

    public ImageUI m_FirstStar;
    public ImageUI m_SecondStar;
    public ImageUI m_ThirdStar;

    public HintEffect m_HammerHint;
    public HintEffect m_AddTurnHint;

    public GameObject m_Info;
    public CanvasGroup m_ResultCanvasGroup;
    public TextUI m_Result;
    public ImageUI m_ResultFirstStar;
    public ImageUI m_ResultSecondStar;
    public ImageUI m_ResultThirdStar;
    public TextUI m_Reason;

    public GameObject m_ContinueButton;

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
        SceneManager.LoadScene(Define.SCENE_SELECT_LEVEL);
    }

    public void Retry()
    {
        m_ResultCanvasGroup.alpha = 0f;
        m_ResultCanvasGroup.interactable = false;
        m_ResultCanvasGroup.blocksRaycasts = false;
        GameManager.instance.ResetGame();
    }

    public void Result(int result, string reason)
    {
        m_ResultCanvasGroup.alpha = 1f;
        m_ResultCanvasGroup.interactable = true;
        m_ResultCanvasGroup.blocksRaycasts = true;

        m_Result.SetText(result.ToString());
        if (DataManager.instance.CheckGoal(result) > 0)
        {
            m_ContinueButton.SetActive(true);
            m_Reason.SetText("Congratulation !!!");
        } 
        else
            m_Reason.SetText(reason);

        DataManager.instance.SaveGame();
    }

    public void ToggleInfo()
    {
        m_Info.SetActive(!m_Info.activeSelf);
    }

    public void SkipToEnd()
    {
        StartCoroutine(GameManager.instance.ForceEndGame());
    }

    public void NextLevel()
    {
        int nextLevel = DataManager.instance.GetNextLevel();
        if (nextLevel == -1)
            SceneManager.LoadScene(Define.SCENE_SELECT_LEVEL);
        else
        {
            PlayerPrefs.SetInt(Define.CURRENT_LEVEL_KEY, nextLevel);
            SceneManager.LoadScene(Define.SCENE_PLAY);
        }
    }

    public void BuyHammer()
    {
        GameManager.instance.BuyHammer();
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
