using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    private static string WARNING_PARAM = "isWarning";

    [SerializeField] private int WarnThreshold = -1;
    [SerializeField] private Animator animator;
    private Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        m_Text.text = text;
    }

    public void SetNumber(int numb)
    {
        if (m_Text.text == numb.ToString())
            return;

        if (numb <= WarnThreshold)
        {
            animator.SetTrigger(WARNING_PARAM);
        }
        m_Text.text = numb.ToString();
    }

    public void SetColor(Color color)
    {
        m_Text.color = color;
    }
}