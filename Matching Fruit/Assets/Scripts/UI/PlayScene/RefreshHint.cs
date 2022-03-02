using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshHint : MonoBehaviour
{
    private ParticleSystem selectedEffect;

    // Start is called before the first frame update
    void Start()
    {
        selectedEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
            selectedEffect.Play();
        else
            selectedEffect.Stop();
    }
}
