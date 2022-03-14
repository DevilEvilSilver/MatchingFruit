using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(PlayButtonSFX);
    }

    private void PlayButtonSFX()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFX_BUTTON);
    }
}
