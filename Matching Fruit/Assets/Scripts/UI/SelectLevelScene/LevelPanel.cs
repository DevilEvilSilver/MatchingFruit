using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelPanel : MonoBehaviour
{
    public static LevelPanel instance;
    
    [SerializeField] private List<Level> m_Levels = new List<Level>();

    public TextUI m_Level;
    public GameObject m_BossImage;
    public TextUI m_Time;
    public TextUI m_Turns;
    public TextUI m_Goal;
    public TextUI m_Highscore;

    public ImageUI m_FirstStar;
    public ImageUI m_SecondStar;
    public ImageUI m_ThirdStar;
    public CanvasGroup m_LevelPanel;

    public Animator m_Fader;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetLevelPanel(int index)
    {
        m_Level.SetText("Level " + (index + 1).ToString());
        if (m_Levels[index].isBossLevel)
            m_BossImage.SetActive(true);
        m_Time.SetText(m_Levels[index].limitedTime.ToString());
        m_Turns.SetText(m_Levels[index].turns.ToString());
        m_Goal.SetText(m_Levels[index].goal_1.ToString());
        m_Highscore.SetText(RecordManager.instance.Progresses[index].score.ToString());
        if (RecordManager.instance.Progresses[index].star >= 1)
        {
            m_FirstStar.SetUnlock(true);
        }
        if (RecordManager.instance.Progresses[index].star >= 2)
        {
            m_SecondStar.SetUnlock(true);
        }
        if (RecordManager.instance.Progresses[index].star >= 3)
        {
            m_ThirdStar.SetUnlock(true);
        }
    }

    public void TurnOffPanel()
    {
        m_LevelPanel.alpha = 0;
        m_LevelPanel.interactable = false;
        m_LevelPanel.blocksRaycasts = false;
    }

    public void Play()
    {
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(Define.SCENE_PLAY);
    }
}
