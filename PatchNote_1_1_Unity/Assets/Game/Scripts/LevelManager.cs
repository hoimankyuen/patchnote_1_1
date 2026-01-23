using System.Collections;
using System.Collections.Generic;
using MoonlightTools.GeneralTools;
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

    public LevelInfo CurrentLevelInfo { get; private set; } = null;
    
    // ======== Unity Events ========
    
    protected override void Awake()
    {
        base.Awake();
        
        RetrieveLevelIndex();
    }

    // ======== Functionalities ========
    
    private void RetrieveLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        CurrentLevelInfo = m_levelLibrary.GetLevelInfoOf(sceneName);
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

        LevelInfo levelInfo = levelNumber == -1 ? null : m_levelLibrary.GetLevelInfo(levelNumber);
        LoadingFader.Instance.Show(() =>
        {
            SceneManager.LoadScene(levelInfo == null ? m_mainMenuSceneName : levelInfo.sceneName, LoadSceneMode.Single);
            CurrentLevelInfo = levelInfo;
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
}
