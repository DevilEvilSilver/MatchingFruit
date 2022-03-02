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

    public void SetText(Sprite sprite)
    {
        m_Sprite.sprite = sprite;
    }
}
