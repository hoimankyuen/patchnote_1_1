using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequirementData
{
    public ItemType itemType;
    public int quantity;

    public RequirementData(ItemType itemType, int quantity)
    {
        this.itemType = itemType;
        this.quantity = quantity;
    }

    public RequirementData(RequirementData other)
    {
        itemType = other.itemType;
        quantity = other.quantity;
    }
    
    public void DecreaseQuantity()
    {
        quantity--;
    }
}

[System.Serializable]
public class LapData
{
    public List<RequirementData> requirements;
}

[CreateAssetMenu(menuName = "Data/Level Data", fileName = "LevelData")]
public class LevelData : ScriptableObject
{
    [NonSerialized] public int number;
    
    [Header("Main Data")]
    public string title;
    public string sceneName;
    public float timeLimit;
    
    [Header("Lap Data")]
    public List<LapData> laps;
}
