using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] protected Sprite originSprite = null;
    [SerializeField] protected Sprite downSprite = null;

    void OnMouseDown()
    {
        if (downSprite != null)
            GetComponent<Image>().sprite = downSprite;
    }

    void OnMouseUp()
    {
        if (originSprite != null)
            GetComponent<Image>().sprite = originSprite;
    }
}
