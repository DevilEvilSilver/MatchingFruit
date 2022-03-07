using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingGroup : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SetActive(bool isAcive)
    {
        canvasGroup.alpha = 1.0f;
        if (isAcive)
        {
            yield break;
        }
        else
        {
            float time = 0f;
            while (time < duration)
            {
                canvasGroup.alpha -= Time.deltaTime / duration;
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
