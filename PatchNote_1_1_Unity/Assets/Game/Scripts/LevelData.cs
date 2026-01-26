using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LapData
{
    public List<ItemGroupQuantity> Requirements;
    public float TimeAllocated;
}

[CreateAssetMenu(menuName = "Data/Level Data", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    [NonSerialized] public int Number;
    
    [Header("Main Data")]
    public string Title;
    public string SceneName;
    
    [Header("Lap Data")]
    public List<LapData> Laps;
}
