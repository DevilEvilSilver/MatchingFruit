using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngameObjectRate
{
    public IngameObject ingameObject;
    public float percentage;
}

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public Texture2D map;

    public int row, col;

    public GameObject prefab;

    public int goal_1;

    public int goal_2;

    public int goal_3;

    public int limitedTime;

    public int turns;

    public int startBalance;

    public float rareObjectsPercentage;

    public List<IngameObject> commmonObjects = new List<IngameObject>();
    public List<IngameObjectRate> rareObjects = new List<IngameObjectRate>();
    public List<ClearMission> clearMissions = new List<ClearMission>();
}
