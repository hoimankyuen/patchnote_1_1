using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LapData
{
    public List<ItemQuantity> Requirements;
}

[CreateAssetMenu(menuName = "Data/Level Data", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    [NonSerialized] public int Number;
    
    [Header("Main Data")]
    public string Title;
    public string SceneName;
    public float TimeLimit;
    
    [Header("Lap Data")]
    public List<LapData> Laps;
}
