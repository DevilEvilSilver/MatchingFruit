using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelScene : MonoBehaviour
{
    public Animator m_Fader;

    void Start()
    {
        AudioManager.instance.PlayBGM(AudioManager.BGM_TITLE);
    }

    public void ResetData()
    {
        StartCoroutine(ResetDataCoroutine());
    }

    public void CheatLevel()
    {
        StartCoroutine(CheatCoroutine());
    }

    private IEnumerator ResetDataCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

        RecordManager.instance.ResetData();
        SceneManager.LoadScene(Define.SCENE_SELECT_LEVEL);
    }

    private IEnumerator CheatCoroutine()
    {
        m_Fader.SetBool(Define.SCENE_FADER_KEY, true);
        yield return new WaitForSeconds(0.5f);

        RecordManager.instance.CheatLevel();
        SceneManager.LoadScene(Define.SCENE_SELECT_LEVEL);
    }

    public void QuitGame()
    {
        RecordManager.instance.SaveData();
        ConfigManager.instance.SaveConfig();
        Application.Quit();
    }
}
