using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level Library", fileName = "LevelLibrary")]
public class LevelLibrary : ScriptableObject
{
    [SerializeField] private List<LevelData> levelInfos;

    public int GetLevelCount()
    {
        return levelInfos.Count;
    }
    
    public LevelData GetLevelInfo(int number)
    {
        levelInfos[number].Number = number;
        return levelInfos[number];
    }

    public LevelData GetLevelInfoOf(string sceneName)
    {
        int number = levelInfos.FindIndex(x => x.SceneName == sceneName);
        return GetLevelInfo(number);
    }
}


