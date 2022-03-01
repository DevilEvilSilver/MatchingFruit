using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private float repeatInterval;

    private Queue<GameObject> spawnQueue;

    // Start is called before the first frame update
    void Start()
    {
        spawnQueue = new Queue<GameObject>();
        if (repeatInterval > 0)
        {
            InvokeRepeating("SpawnObject", 0.0f, repeatInterval);
        }
    }

    public void AddSpawn(GameObject gameObject)
    {
        spawnQueue.Enqueue(gameObject);
    }

    private GameObject SpawnObject()
    {
        if (spawnQueue.Count > 0)
        {
            return Instantiate(spawnQueue.Dequeue(), transform.position, Quaternion.identity);
        }
        return null;
    }
}
