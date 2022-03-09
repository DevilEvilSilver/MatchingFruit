using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Button button;
    private int m_Score = 0;
    public ImageUI m_FirstStar;
    public ImageUI m_SecondStar;
    public ImageUI m_ThirdStar;
    public CanvasGroup m_LevelPanel;

    void Start()
    {
        LevelProgress currProgress = RecordManager.instance.Progresses[index];
        if(!currProgress.isUnlock)
            button.interactable = false;

        m_Score = currProgress.score;
        if (currProgress.star >= 1)
        {
            m_FirstStar.SetUnlock(true);
        }
        if (currProgress.star >= 2)
        {
            m_SecondStar.SetUnlock(true);
        }
        if (currProgress.star >= 3)
        {
            m_ThirdStar.SetUnlock(true);
        }
    }

    public void SelectLevel()
    {
        PlayerPrefs.SetInt(Define.CURRENT_LEVEL_KEY, index  );
        LevelPanel.instance.SetLevelPanel(index);
        m_LevelPanel.alpha = 1;
        m_LevelPanel.interactable = true;
        m_LevelPanel.blocksRaycasts = true;
    }
}
