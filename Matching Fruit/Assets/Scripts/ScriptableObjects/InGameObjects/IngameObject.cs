using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Object")]
public class IngameObject : ScriptableObject
{
    public Sprite sprite;

    public enum ObjectType
    {
        Char_1, Char_2, Char_3, Char_4, Char_5, Char_6, Char_7,
        Bomb, Rainbow, Clock
    }
    public ObjectType type;

    public bool isRare = true;
}
