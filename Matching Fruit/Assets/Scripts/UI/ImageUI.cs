using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageUI : MonoBehaviour
{
    private Image m_Sprite;

    // Start is called before the first frame update
    void Start()
    {
        m_Sprite = GetComponent<Image>();
    }

    public void SetSprite(Sprite sprite)
    {
        m_Sprite.overrideSprite = sprite;
    }

    public void SetUnlock(bool isUnlock)
    {
        if (isUnlock)
            m_Sprite.color = Color.white;
        else 
            m_Sprite.color = Color.black;
    }
}
