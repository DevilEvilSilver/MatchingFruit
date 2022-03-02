using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLane : MonoBehaviour
{
    private LayerMask m_LayerMask;
    private Vector3 m_ObjectSize;

    // Start is called before the first frame update
    void Start()
    {
        m_LayerMask = LayerMask.GetMask("Blocking");
        m_ObjectSize = GetComponent<Collider2D>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCanMatching();
    }

    private void CheckCanMatching()
    {
        if (Physics2D.OverlapArea(new Vector2(transform.position.x + m_ObjectSize.x / 2, transform.position.y + m_ObjectSize.y / 2),
            new Vector2(transform.position.x - m_ObjectSize.x / 2, transform.position.y - m_ObjectSize.y / 2), m_LayerMask))
            Matrix.instance.m_CanMatching = false;
        else
        {
            Matrix.instance.m_CanMatching = true;
        }   
    }
}
