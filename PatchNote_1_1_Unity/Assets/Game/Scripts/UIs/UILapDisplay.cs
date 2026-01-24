using System.Collections;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILapDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_frame;
    [SerializeField] private TextMeshProUGUI m_text;
    
    [Header("Settings")] 
    [SerializeField] private float m_blinkInterval;
    [SerializeField] private int m_blinkTimes;
    [SerializeField] private Color m_normalTextColour;
    [SerializeField] private Color m_blinkTextColour;
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;

    private Coroutine m_blinkCoroutine;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_gameManager.CurrentLapChanged += OnCurrentLapChanged;
        OnCurrentLapChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.LevelTimer.RemainingTimeChanged -= OnCurrentLapChanged;
        }
    }

    private void Update()
    {
        ShowHide();
    }
    
    // ======== Other Events ========

    private void OnCurrentStateChanged()
    {
        m_targetAlpha = m_gameManager.CurrentState
            is GameManager.State.Playing
            or GameManager.State.Paused 
            ? 1f : 0f;
    }

    private void OnCurrentLapChanged()
    {
        ShowLapText();
        BlinkText();
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }
    
    private void ShowLapText()
    {
        m_text.text = $"LAP {m_gameManager.CurrentLap}/{LevelManager.Instance.CurrentLevelData.laps.Count}";
    }

    private void BlinkText()
    {
        if (m_blinkCoroutine != null)
        {
            StopCoroutine(m_blinkCoroutine);
            m_blinkCoroutine = null;
        }
        m_blinkCoroutine = StartCoroutine(BlinkTextSequence());
    }

    private IEnumerator BlinkTextSequence()
    {
        for (int i = 0 ; i < m_blinkTimes; i++)
        {
            m_text.color = m_blinkTextColour;
            yield return new WaitForSeconds(m_blinkInterval / 2f);
            m_text.color = m_normalTextColour;
            yield return new WaitForSeconds(m_blinkInterval / 2f);
        }
        m_blinkCoroutine = null;
    }
}
