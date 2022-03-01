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
            firstSelected.isSelected = false;
            firstSelected = null;
            return;
        }
        else
        {
            if (firstSelected == null)
            {
                firstSelected = objectClicked;
                firstSelected.isSelected = true;
                return;
            }
            else
            {
                objectClicked.isSelected = true;
                Swap(objectClicked);
            }
        }
    }

    private static void Swap(Object secondSelected)
    {
        if (GameManager.instance.Swap(firstSelected.gameObject, secondSelected.gameObject))
        {
            firstSelected.isSelected = false;
            secondSelected.isSelected = false;
            firstSelected = null;
        }
        else
        {
            firstSelected.isSelected = false;
            firstSelected = secondSelected;
        }
    }
}
