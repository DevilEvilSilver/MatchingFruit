using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    private ParticleSystem selectedEffect;
    private Rigidbody2D rb2D;
    internal IngameObject properties;

    void Start()
    {
        selectedEffect = GetComponentInChildren<ParticleSystem>();
        rb2D = GetComponentInChildren<Rigidbody2D>();
    }

    void Update()
    {
        if (rb2D.velocity.y < -300f)
            rb2D.velocity = new Vector2(0, -300f);
    }

    void OnMouseDown()
    {
        Matrix.instance.ObjectClicked(this);
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
