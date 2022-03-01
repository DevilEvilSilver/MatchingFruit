using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [SerializeField] private float repeatInterval;

    private Queue<GameObject> spawnQueue;
    private Queue<Vector2Int> matrixPos;

    // Start is called before the first frame update
    void Start()
    {
        spawnQueue = new Queue<GameObject>();
        matrixPos = new Queue<Vector2Int>();
        if (repeatInterval > 0)
        {
            InvokeRepeating("SpawnObject", 0.0f, repeatInterval);
        }
    }

    public void AddSpawn(GameObject gameObject, Vector2Int pos)
    {
        spawnQueue.Enqueue(gameObject);
        matrixPos.Enqueue(pos);  
    }

    private void SpawnObject()
    {
        if (spawnQueue.Count > 0)
        {
            Matrix.instance.SetObjectToMatrix(Instantiate(spawnQueue.Dequeue(), transform.position, Quaternion.identity), matrixPos.Dequeue());
        }
    }
}
