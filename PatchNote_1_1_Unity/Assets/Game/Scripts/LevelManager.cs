using System.Collections;
using MoonlightTools.GeneralTools;
using MoonlightTools.StoredDataSystem;
using MoonlightTools.UIToolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviourPersistentSingleton<LevelManager>
{
    [Header("Settings")] 
    [SerializeField] private string m_mainMenuSceneName;
    [SerializeField] private LevelLibrary m_levelLibrary;
    [SerializeField] private float m_loadSceneFaderDelay;
    
    private bool m_loadingScene = false;
    
    public int UnlockedLevel { get; private set; }
    
    public LevelData CurrentLevelData { get; private set; }
    
    public bool HasNextLevel => CurrentLevelData != null && CurrentLevelData.Number < m_levelLibrary.GetLevelCount() - 1;
    
    // ======== Unity Events ========
    
    protected override void Awake()
    {
        base.Awake();
        
        RetrieveLevelIndex();
    }

    private void Start()
    {
        RetrieveInitialUnlockedLevels();
    }

    // ======== Functionalities ========

    private void RetrieveLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        CurrentLevelData = m_levelLibrary.GetLevelInfoOf(sceneName);
    }

    public void GotoMainMenu()
    {
        GotoLevel(-1);
    }

    public void GotoLevel(int levelNumber)
    {
        if (m_loadingScene)
            return;

        m_loadingScene = true;

        LevelData levelInfo = levelNumber == -1 ? null : m_levelLibrary.GetLevelInfo(levelNumber);
        LoadingFader.Instance.Show(() =>
        {
            SceneManager.LoadScene(levelInfo == null ? m_mainMenuSceneName : levelInfo.SceneName, LoadSceneMode.Single);
            CurrentLevelData = levelInfo;
            StartCoroutine(DelayedHideFader());
        });
    }
    
    private IEnumerator DelayedHideFader()
    {
        yield return new WaitForSeconds(m_loadSceneFaderDelay);
        LoadingFader.Instance.Hide(() =>
        {
            m_loadingScene = false;
        });
    }
    
    // ======= Level Save Data ========

    private void RetrieveInitialUnlockedLevels()
    {
        UnlockedLevel = StoredDataManager.Instance.GetOrSetupInt(DataCategory.Shared, $"UnlockedLevel", 0);
        StoredDataManager.Instance.Save(DataCategory.Shared);
    }

    public void UnlockNextLevel()
    {
        if (CurrentLevelData == null)
            return;
        
        StoredDataManager.Instance.SetInt(DataCategory.Shared, $"UnlockedLevel", CurrentLevelData.Number + 1);
        StoredDataManager.Instance.Save(DataCategory.Shared);
    }
    
    public void SaveCurrentLevelResults(float score, float time, bool gemFound, bool jamFound, bool goldFound)
    {
        if (CurrentLevelData == null)
            return;
        
        LoadLevelResults(CurrentLevelData.Number, out float prevScore,  out float prevTime,  out bool prevGemFound,  out bool prevJamFound, out bool prevGoldFound);
        LevelResults results = new LevelResults(
             score >= prevScore ? score : prevScore, 
             score >= prevScore ? time : time <= prevTime ? time : prevTime,
            gemFound || prevGemFound,
            jamFound || prevJamFound,
            goldFound || prevGoldFound);
        
        string json = JsonUtility.ToJson(results);
        StoredDataManager.Instance.SetString(DataCategory.Shared, $"LevelResult_{CurrentLevelData.Number}", json);
        StoredDataManager.Instance.Save(DataCategory.Shared);
    }

    public void LoadLevelResults(int levelNumber,out float score,out float time,out bool gemFound,out bool jamFound,out bool goldFound)
    {
        score = 0;
        time = 0;
        gemFound = false;
        jamFound = false;
        goldFound = false;
        string json = StoredDataManager.Instance.GetString(DataCategory.Shared, $"LevelResult_{levelNumber}");
        if (!string.IsNullOrEmpty(json))
        {
            LevelResults results = JsonUtility.FromJson<LevelResults>(json);
            score = results.Score;
            time = results.Time;
            gemFound = results.GemFound;
            jamFound = results.JamFound;
            goldFound = results.GoldFound;
        }
    }
}

    
[System.Serializable]
public class LevelResults
{
    public float Score;
    public float Time;
    public bool GemFound;
    public bool JamFound;
    public bool GoldFound;

    public LevelResults(float score, float time, bool gemFound, bool jamFound, bool goldFound)
    {
        Score = score;
        Time = time;
        GemFound = gemFound;
        JamFound = jamFound;
        GoldFound = goldFound;
    }
}
