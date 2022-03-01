using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanic : MonoBehaviour
{
    private static Object firstSelected = null;

    public static void ObjectClicked(Object objectClicked)
    {
        if (firstSelected == objectClicked)
        {
            firstSelected.SetSelected(false);
            firstSelected = null;
            return;
        }
        else
        {
            if (firstSelected == null)
            {
                firstSelected = objectClicked;
                firstSelected.SetSelected(true);
                return;
            }
            else
            {
                objectClicked.SetSelected(true);
                Swap(objectClicked);
            }
        }
    }

    private static void Swap(Object secondSelected)
    {
        if (GameManager.instance.Swap(firstSelected.gameObject, secondSelected.gameObject))
        {
            firstSelected.SetSelected(false);
            secondSelected.SetSelected(false);
            firstSelected = null;
        }
        else
        {
            firstSelected.SetSelected(false);
            firstSelected = secondSelected;
        }
    }
}
