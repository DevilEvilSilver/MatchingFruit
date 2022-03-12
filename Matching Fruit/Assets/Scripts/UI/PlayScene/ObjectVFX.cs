using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectVFX : MonoBehaviour
{
    private static string ACTIVE_EFFECT_PARAM = "isActiveEffect";

    [SerializeField] private Animator m_Destroyed;
    [SerializeField] private Animator m_BombEffect;
    [SerializeField] private Animator m_LightningEffect;
    [SerializeField] private Animator m_HammerEffect;
    [SerializeField] private FadingText m_ClockEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveDestroy()
    {
        m_Destroyed.SetTrigger(ACTIVE_EFFECT_PARAM);
    }

    public void ActiveBomb()
    {
        m_BombEffect.SetTrigger(ACTIVE_EFFECT_PARAM);
    }

    public void ActiveLightning()
    {
        m_LightningEffect.SetTrigger(ACTIVE_EFFECT_PARAM);
    }
    public void ActiveClock(string text)
    {
        StartCoroutine(m_ClockEffect.SetFadingText(text));
    }

    public void ActiveHammer()
    {
        m_HammerEffect.SetTrigger(ACTIVE_EFFECT_PARAM);
    }
}
