using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;

    private Queue<GameObject> spawnQueue;
    private float coolDown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnQueue = new Queue<GameObject>();
    }

    void Update()
    {
        coolDown -= Time.deltaTime;
    }

    public void AddSpawn(GameObject gameObject)
    {
        spawnQueue.Enqueue(gameObject);
    }

    public GameObject SpawnObject()
    {
        if (spawnQueue.Count != 0 && coolDown < Mathf.Epsilon)
        {
            return Instantiate(spawnQueue.Dequeue(), transform.position, Quaternion.identity);
            coolDown = 0.5f;
        }
        return null;
    }
}
