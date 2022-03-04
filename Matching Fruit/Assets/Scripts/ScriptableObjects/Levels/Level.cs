using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngameObbjectRate
{
    public IngameObject ingameObject;
    public float percentage;
}

[CreateAssetMenu(menuName = "Level")]
public class Level : ScriptableObject
{
    public int goal_1;

    public int goal_2;

    public int goal_3;

    public int limitedTime;

    public int turns;

    public float rareObjectsPercentage;

    public List<IngameObbjectRate> commmonObjects = new List<IngameObbjectRate>();
    public List<IngameObbjectRate> rareObjects = new List<IngameObbjectRate>();
    public List<ClearMission> clearMissions = new List<ClearMission>();
}
