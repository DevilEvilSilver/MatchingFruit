using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelScene : MonoBehaviour
{
    public void SelectLevel(int level)
    {
        PlayerPrefs.SetInt(Define.CURRENT_LEVEL_KEY, level - 1);
        SceneManager.LoadScene(Define.SCENE_PLAY);
    }
}
