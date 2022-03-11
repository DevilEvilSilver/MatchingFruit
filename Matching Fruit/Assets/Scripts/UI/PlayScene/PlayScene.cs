using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayScene : MonoBehaviour
{
    public static PlayScene instance;

    public TextUI m_LevelText;

    public FadingGroup m_Message;
    public FadingText m_Combo;
    public TextUI m_Score;
    public TextUI m_Turns;
    public TextUI m_Time;
    public TextUI m_Balance;
    public TextUI m_MissionCounter;
    public ImageUI m_Mission;

    public Slider m_ScoreSlider;
    public float m_ScoreSliderHeight;
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

    public Animator m_Fader;

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
        StartCoroutine(QuitCoroutine());
    }

    private IEnumerator QuitCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(Define.SCENE_SELECT_LEVEL);
    }

    public void Retry()
    {
        StartCoroutine(RetryCoroutine());
    }

    private IEnumerator RetryCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

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
        {
            m_ContinueButton.SetActive(false);
            m_Reason.SetText(reason);
        }

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
        StartCoroutine(NextLevelCoroutine());
    }

    public void SetStarPos(int goal1, int goal2, int goal3)
    {
        Vector3 pos1 = m_FirstStar.transform.position;
        pos1.y += (float)goal1 / goal3 * m_ScoreSliderHeight;
        m_FirstStar.transform.position = pos1;

        Vector3 pos2 = m_SecondStar.transform.position;
        pos2.y += (float)goal2 / goal3 * m_ScoreSliderHeight;
        m_SecondStar.transform.position = pos2;

        Vector3 pos3 = m_ThirdStar.transform.position;
        pos3.y += m_ScoreSliderHeight;
        m_ThirdStar.transform.position = pos3;
    }

    private IEnumerator NextLevelCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

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
