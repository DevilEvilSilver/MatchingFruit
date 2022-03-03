using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintEffect: MonoBehaviour
{
    private ParticleSystem hintEffect;

    // Start is called before the first frame update
    void Start()
    {
        hintEffect = GetComponentInChildren<ParticleSystem>();
    }
    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            hintEffect.Play();
        else
            hintEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
