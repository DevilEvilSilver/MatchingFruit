using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Button button;
    public int m_Score = 0;
    public ImageUI m_FirstStar;
    public ImageUI m_SecondStar;
    public ImageUI m_ThirdStar;

    void Start()
    {
        LevelProgress currProgress = LevelManager.instance.Progresses[index];
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
        PlayerPrefs.SetInt(Define.CURRENT_LEVEL_KEY, index);
        SceneManager.LoadScene(Define.SCENE_PLAY);
    }
}
