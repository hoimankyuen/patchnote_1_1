using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfo
{
    [NonSerialized] public int number;
    public string title;
    public string sceneName;
    public float timeLimit;
}

[CreateAssetMenu(menuName = "Data/Level Library", fileName = "Level Library")]
public class LevelLibrary : ScriptableObject
{
    [SerializeField] private List<LevelInfo> levelInfos;

    public int GetLevelCount()
    {
        return levelInfos.Count;
    }
    
    public LevelInfo GetLevelInfo(int number)
    {
        levelInfos[number].number = number;
        return levelInfos[number];
    }

    public LevelInfo GetLevelInfoOf(string sceneName)
    {
        return levelInfos.Find(level => level.sceneName == sceneName);
    }
}


