using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    private ParticleSystem selectedEffect;

    void Start()
    {
        selectedEffect = GetComponentInChildren<ParticleSystem>();
    }

    void OnMouseDown()
    {
        Mechanic.ObjectClicked(this);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            selectedEffect.Play();
        else
            selectedEffect.Stop();
    }
}
