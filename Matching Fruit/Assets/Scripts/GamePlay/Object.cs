using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField] private float m_Gravity = -250f;
    private Vector2 m_Velocity = Vector2.zero;
    private Vector3 m_MatrixPosition;
    internal Vector2Int m_MatrixIndex;

    private ParticleSystem selectedEffect;
    internal IngameObject properties;
    private Rigidbody2D rb2D;

    void Start()
    {
        selectedEffect = GetComponentInChildren<ParticleSystem>();
        rb2D = GetComponentInChildren<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (transform.position.y > m_MatrixPosition.y)
        {
            m_Velocity.y = m_Velocity.y + m_Gravity * Time.deltaTime;
            Matrix.instance.UpdateFallingObject(m_MatrixIndex, true);
        }
        else
        {
            m_Velocity.y = 0f;
            transform.position = m_MatrixPosition;
            Matrix.instance.UpdateFallingObject(m_MatrixIndex, false);
        }
        rb2D.velocity = m_Velocity;
    }

    void OnMouseDown()
    {
        Matrix.instance.ObjectClicked(this);
    }

    public void SetMatrixPosition(Vector3 pos)
    {
        m_MatrixPosition = pos;
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            selectedEffect.Play();
        else
            selectedEffect.Stop();
    }

    public void SetObjectProperties(IngameObject properties)
    {
        this.properties = properties;
        GetComponent<SpriteRenderer>().sprite = properties.sprite;
    }
}
