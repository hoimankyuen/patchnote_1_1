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
    [SerializeField] private List<string> m_levelSceneNames;
    [SerializeField] private float m_loadSceneFaderDelay;
    
    private bool m_loadingScene = false;

    public int CurrentLevelNumber { get; private set; } = -1;
    
    // ======== Unity Events ========
    
    private void Start()
    {
        RetrieveLevelIndex();
    }

    // ======== Functionalities ========
    
    private void RetrieveLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        CurrentLevelNumber = m_levelSceneNames.IndexOf(sceneName);
    }
    
    public void GotoMainMenu()
    {
        GotoLevel(0);
    }

    public void GotoLevel(int levelNumber)
    {
        if (m_loadingScene)
            return;

        m_loadingScene = true;
        
        string sceneName = levelNumber == -1 ? m_mainMenuSceneName : m_levelSceneNames[levelNumber];
        LoadingFader.Instance.Show(() =>
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            CurrentLevelNumber = levelNumber;
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
