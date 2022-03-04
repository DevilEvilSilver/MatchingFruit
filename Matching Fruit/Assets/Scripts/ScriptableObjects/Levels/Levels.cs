using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class Levels : ScriptableObject
{
    public int goal;

    public int limitedTime;

    public int turns;

    public float rareObjectsPercentage;
}
