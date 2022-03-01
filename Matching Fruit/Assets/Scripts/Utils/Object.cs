using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    private ParticleSystem selectedEffect;
    internal IngameObject.ObjectType type;

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
        type = properties.type;
        GetComponent<SpriteRenderer>().sprite = properties.sprite;
    }
}
