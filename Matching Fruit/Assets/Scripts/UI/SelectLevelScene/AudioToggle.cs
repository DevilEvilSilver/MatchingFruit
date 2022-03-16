using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{ 
    [SerializeField] private Image m_Image;
    [SerializeField] private Sprite m_ActiveSprite;
    [SerializeField] private Sprite m_InactiveSprite;
    private bool m_IsActive = true;

    // Start is called before the first frame update
    void Start()
    {
        if (ConfigManager.instance.config.isMute)
            m_IsActive = true;
        else
            m_IsActive = false;
        Toggle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
    {
        if (m_IsActive)
        {
            m_IsActive = false;
            m_Image.sprite = m_InactiveSprite;
        }
        else
        {
            m_IsActive = true;
            m_Image.sprite = m_ActiveSprite;
        }

        AudioManager.instance.ToggleBGMVolume(m_IsActive);
        AudioManager.instance.ToggleSFXVolume(m_IsActive);
        ConfigManager.instance.config.isMute = !m_IsActive;
        ConfigManager.instance.SaveConfig();
    }
}
