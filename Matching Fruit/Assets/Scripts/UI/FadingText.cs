using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : TextUI
{
    [SerializeField] private float duration = 2f;
    private Color m_TextColor;

    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();
        m_TextColor = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        m_TextColor.a -= Time.deltaTime / duration;
        m_Text.color = m_TextColor;
    }

    public override void SetText(string text)
    {
        m_Text.text = text;
        m_TextColor = new Color(1, 1, 1, 1);
    }
}
