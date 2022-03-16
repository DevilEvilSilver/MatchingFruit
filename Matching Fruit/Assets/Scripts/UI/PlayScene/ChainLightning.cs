using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChainLightning : MonoBehaviour{
    [Header("Prefabs")]
    public GameObject lineRendererPrefab;
    public GameObject lightRendererPrefab;

    [Header("Config")]
    public float offsetSize;

    private Vector2 source;
    private int lightnings = 5;
    private float segmentLength = 20f;
    
    private LightningBolt[,] LightningBolts{get; set;}
    private Vector2[,] Targets{get; set;}
    
    void Awake()
    {
        LightningBolts = new LightningBolt[Matrix.instance.Row, Matrix.instance.Column];
        Targets = new Vector2[Matrix.instance.Row, Matrix.instance.Column];
        source = new Vector2(transform.position.x, transform.position.y);

        LightningBolt tmpLightningBolt;
        for (int i = 0; i < Matrix.instance.Row; i++)
            for (int j = 0; j < Matrix.instance.Column; j++)
        { 
            tmpLightningBolt = new LightningBolt(segmentLength, i * j + j);
            tmpLightningBolt.Init(lightnings, offsetSize, lineRendererPrefab, lightRendererPrefab, transform);
            LightningBolts[i, j] = tmpLightningBolt;

            Vector3 pos = Matrix.instance.transform.position;
            pos.x += j * Matrix.instance.ObjectSize.x - Matrix.instance.ObjectSize.x * (Matrix.instance.Column - 1) / 2;
            pos.y += i * Matrix.instance.ObjectSize.y - Matrix.instance.ObjectSize.y * (Matrix.instance.Row - 1) / 2;
            Targets[i, j] = new Vector2(pos.x, pos.y);
        }
    }
    
    public IEnumerator ChainEffect(int i, int j)
    {
        LightningBolts[i, j].Activate();
        float time = 0.5f;
        while (time > 0f){
            LightningBolts[i, j].DrawLightning(source, Targets[i, j]);
            time -= Time.deltaTime;
            yield return null;
        }
        LightningBolts[i, j].Deactivate();
    }
}