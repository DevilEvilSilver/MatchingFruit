using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private Color textColor = new Color(1, 1, 1, 1);
    [SerializeField] private Vector2 speed = Vector2.zero;
    private Text m_Text;
    private Vector3 m_OriginalPos;
    private Vector3 m_Pos;

    // Start is called before the first frame update
    void Start()
    {
        m_Text = GetComponent<Text>();
        m_Text.color = new Color(1, 1, 1, 0);
        m_OriginalPos = transform.position;
        m_Pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SetFadingText(string text)
    {
        float time = duration;
        m_Pos = m_OriginalPos;
        m_Text.text = text;
        m_Text.color = textColor;
        Color color = textColor;

        while (time > 0)
        {
            color.a -= Time.deltaTime / duration;
            m_Text.color = color;
            m_Pos.x += speed.x * Time.deltaTime;
            m_Pos.y += speed.y * Time.deltaTime;
            transform.position = m_Pos;

            time -= Time.deltaTime;
            yield return null;
        }
    }
}
