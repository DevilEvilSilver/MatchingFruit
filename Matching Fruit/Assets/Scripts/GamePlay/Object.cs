using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField] private float m_Gravity = -250f;
    [SerializeField] private AnimationCurve m_SwapCurve;

    [SerializeField] private SpriteRenderer stateRenderer;
    [SerializeField] private Sprite m_Block;
    [SerializeField] private Sprite m_Chain;
    [SerializeField] private Sprite m_Freeze;

    internal Vector2 m_Velocity = Vector2.zero;
    internal Vector3 m_MatrixPosition;
    internal Vector2Int m_MatrixIndex;
    internal float m_SlideStartPos = 1080f;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem particleEffect;
    private ParticleSystem.MainModule particleMainModule;

    private IngameObject properties;
    internal IngameObject Properties
    {
        get
        {
            return this.properties;
        }
        set
        {
            this.properties = value;
            Mechanic.instance.SetHint(false);
        }
    }

    private Rigidbody2D rb2D;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleEffect = GetComponentInChildren<ParticleSystem>();
        particleMainModule = particleEffect.main;
        rb2D = GetComponentInChildren<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (transform.position.y > m_MatrixPosition.y)
        {
            m_Velocity.y = m_Velocity.y + m_Gravity * Time.deltaTime;
            if (transform.position.x > m_MatrixPosition.x && transform.position.y < m_SlideStartPos)
                m_Velocity.x = m_Velocity.y;
            else if (transform.position.x < m_MatrixPosition.x && transform.position.y < m_SlideStartPos)
                m_Velocity.x = -m_Velocity.y;
            Matrix.instance.UpdateFallingObject(m_MatrixIndex, true);
        }
        else
        {
            m_Velocity = Vector2.zero;
            transform.position = m_MatrixPosition;
            m_SlideStartPos = 1080f;
            Matrix.instance.UpdateFallingObject(m_MatrixIndex, false);
        }
        rb2D.velocity = m_Velocity;
    }

    void OnMouseDown()
    {
        Mechanic.instance.ObjectClicked(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            particleMainModule.startColor = new ParticleSystem.MinMaxGradient(Color.white);
            particleEffect.Play();
        }
        else
        {
            particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void SetHint(bool isHint)
    {
        if (isHint)
        {
            particleMainModule.startColor = new ParticleSystem.MinMaxGradient(Color.green);
            particleEffect.Play();
        }
        else
        {
            particleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public IEnumerator SetWarn()
    {
        int count = 2;
        while (count-- > 0)
        {
            spriteRenderer.color = Color.red;

            yield return new WaitForSeconds(0.3f);

            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetObjectProperties(IngameObject properties)
    {
        this.Properties = properties;
        GetComponent<SpriteRenderer>().sprite = properties.sprite;
    }

    public void SetStateNormal()
    {
        stateRenderer.color = new Color(1, 1, 1, 0);
    }

    public void SetStateBlock()
    {
        stateRenderer.sprite = m_Block;
        stateRenderer.color = Color.white;
    }

    public void SetStateChained()
    {
        stateRenderer.sprite = m_Chain;
        stateRenderer.color = Color.white;
    }

    public void SetStateFreeze()
    {
        stateRenderer.sprite = m_Freeze;
        stateRenderer.color = new Color(0.1f, 0.3f, 1f, 0.5f);
    }

    public IEnumerator MoveTo(Vector3 destiny, float time)
    {
        float currTime = time;
        Vector3 originalPos = transform.position;
        while (currTime > 0)
        {
            transform.position = Vector3.Lerp(destiny, originalPos, m_SwapCurve.Evaluate(currTime / time));
            currTime -= Time.deltaTime;
            yield return null;
        }
        transform.position = Vector3.Lerp(destiny, originalPos, 0);
    }
}
