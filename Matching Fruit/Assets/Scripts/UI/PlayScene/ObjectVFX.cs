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
    [SerializeField] private GameObject m_RainbowEffectPrefab;
    private Animator[,] m_RainbowEffects;

    // Start is called before the first frame update
    void Start()
    {
        //m_RainbowEffects = new Animator[Matrix.instance.Row, Matrix.instance.Column];
        //for (int i = 0; i < Matrix.instance.Row; i++)
        //{
        //    for (int j = 0; j < Matrix.instance.Column; j++)
        //    {
        //        // init object
        //        Vector3 pos = transform.position;
        //        pos.x += j * Matrix.instance.ObjectSize.x - Matrix.instance.ObjectSize.x * (Matrix.instance.Column - 1) / 2;
        //        pos.y += i * Matrix.instance.ObjectSize.y - Matrix.instance.ObjectSize.y * (Matrix.instance.Row - 1) / 2;
        //        var points = new Vector3[2];
        //        points[0] = transform.position;
        //        points[1] = pos;

        //        var rainbowEffObject = Instantiate(m_RainbowEffectPrefab, transform.position, Quaternion.identity);
        //        m_RainbowEffects[i, j] = rainbowEffObject.GetComponent<Animator>();
        //        m_RainbowEffects[i, j].transform.parent = transform;
        //        rainbowEffObject.GetComponent<LineRenderer>().SetPositions(points);
        //    }
        //}
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

    public void ActiveRainbow(int i, int j)
    {
        //m_RainbowEffects[i, j].SetTrigger(ACTIVE_EFFECT_PARAM);
    }

    public void ActiveHammer()
    {
        m_HammerEffect.SetTrigger(ACTIVE_EFFECT_PARAM);
    }
}
