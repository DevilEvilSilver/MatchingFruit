using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    private ParticleSystem selectedEffect;
    internal IngameObject properties;

    void Start()
    {
        selectedEffect = GetComponentInChildren<ParticleSystem>();
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
