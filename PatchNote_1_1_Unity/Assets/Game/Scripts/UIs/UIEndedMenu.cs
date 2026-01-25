using System.Collections;
using MoonlightTools.UIComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEndedMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private Panel m_panel;
    [SerializeField] private TextMeshProUGUI m_titleText;
    
    [SerializeField] private TextMeshProUGUI m_itemLabel;
    [SerializeField] private TextMeshProUGUI m_itemValue;
    [SerializeField] private TextMeshProUGUI m_timeLabel;
    [SerializeField] private TextMeshProUGUI m_timeValue;
    [SerializeField] private TextMeshProUGUI m_scoreLabel;
    [SerializeField] private TextMeshProUGUI m_scoreValue;
    [SerializeField] private TextMeshProUGUI m_bonusLabel;
    [SerializeField] private TextMeshProUGUI m_bonusValue;
    
    [SerializeField] private TextMeshProUGUI m_tryAgainText;
    [SerializeField] private Button m_nextButton;
    [SerializeField] private Button m_retryButton;
    [SerializeField] private Button m_menuButton;
    
    [Header("Settings")]
    [SerializeField] private float m_waitDuration;
    [SerializeField] private float m_beforeTitleDuration;
    [SerializeField] private float m_betweenElementDuration;
    [SerializeField] private float m_beforeButtonDuration;
    
    // ======== Unity Events ========
    
    private void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
    }

    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }
    }
    
    // ======== Other Events ========

    private void OnCurrentStateChanged()
    {
        if (m_gameManager.CurrentState != GameManager.State.Ended)
            return;

        SetupUI();
        StartCoroutine(ShowSequence());
    }

    // ======== Functionalities ========
    
    private void SetupUI()
    {
        m_titleText.gameObject.SetActive(false);
        
        m_itemLabel.gameObject.SetActive(false);
        m_itemValue.gameObject.SetActive(false);
        m_timeLabel.gameObject.SetActive(false);
        m_timeValue.gameObject.SetActive(false);
        m_scoreLabel.gameObject.SetActive(false);
        m_scoreValue.gameObject.SetActive(false);
        m_bonusLabel.gameObject.SetActive(false);
        m_bonusValue.gameObject.SetActive(false);
        
        m_tryAgainText.gameObject.SetActive(false);
        m_nextButton.gameObject.SetActive(false);
        m_retryButton.gameObject.SetActive(false);
        m_menuButton.gameObject.SetActive(false);
    }
    
    private IEnumerator ShowSequence()
    {
        yield return new WaitForSecondsRealtime(m_waitDuration);
        m_panel.Show(true);
        
        yield return new WaitForSecondsRealtime(m_beforeTitleDuration);
        m_titleText.text = m_gameManager.LevelCompleted ? "FINISHED!" : "TIME OUT";
        m_titleText.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(m_betweenElementDuration);
        m_itemValue.text = m_gameManager.CurrentItemCount.ToString();
        m_itemLabel.gameObject.SetActive(true);
        m_itemValue.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(m_betweenElementDuration);
        float time = LevelManager.Instance.CurrentLevelData.TimeLimit - m_gameManager.LevelTimer.RemainingTime;
        m_timeValue.text = $"{(int)(time/60):D2}:{(int)(time%60):D2}:{(int)(time*100%100):D2}";
        m_timeLabel.gameObject.SetActive(true);
        m_timeValue.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(m_betweenElementDuration);
        m_scoreValue.text = m_gameManager.CurrentScore.ToString();
        m_scoreLabel.gameObject.SetActive(true);
        m_scoreValue.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(m_betweenElementDuration);
        int bonus = (m_gameManager.GemFound ? 1 : 0) + (m_gameManager.JamFound ? 1 : 0) + (m_gameManager.GoldFound ? 1 : 0);
        m_bonusValue.text = $"{bonus}/3";
        m_bonusLabel.gameObject.SetActive(true);
        m_bonusValue.gameObject.SetActive(true);
        
        yield return new WaitForSecondsRealtime(m_beforeButtonDuration);
        m_tryAgainText.gameObject.SetActive(!m_gameManager.LevelCompleted);
        m_nextButton.gameObject.SetActive(m_gameManager.LevelCompleted && LevelManager.Instance.HasNextLevel);
        m_retryButton.gameObject.SetActive(true);
        m_menuButton.gameObject.SetActive(true);
        if (m_nextButton.gameObject.activeInHierarchy)
        {
            m_nextButton.Select();
        }
        else
        {
            m_retryButton.Select();
        }
    }

    public void NextLevel()
    {
        m_gameManager.NextLevel();
    }

    public void RestartLevel()
    {
        m_gameManager.Restart();
    }

    public void BackToMenu()
    {
        m_gameManager.BackToManuMenu();
    }
}
