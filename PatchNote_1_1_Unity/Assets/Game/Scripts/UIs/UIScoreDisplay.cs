using System.Collections;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_frame;
    [SerializeField] private TextMeshProUGUI m_text;

    [Header("Settings")]
    [SerializeField] private float m_scoreChangeDuration;    
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;

    private float m_currentScore = 0f;
    private Coroutine m_changePriceCoroutine;
    
    // ======== Unity Events ========
    
    public void Start()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_gameManager.CurrentScoreChanged += OnCurrentScoreChanged;
        OnCurrentScoreChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.CurrentScoreChanged -= OnCurrentScoreChanged;
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

    private void OnCurrentScoreChanged()
    {
        ChangePrice(m_gameManager.CurrentScore);
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }
    
    
    private void ChangePrice(float score)
    {
        if (m_changePriceCoroutine != null)
        {
            StopCoroutine(m_changePriceCoroutine);
            m_changePriceCoroutine = null;
        }
        m_changePriceCoroutine = StartCoroutine(ChangePriceSequence(score));
        
        m_text.text = $"LAP {m_gameManager.CurrentLap}/{LevelManager.Instance.CurrentLevelData.laps.Count}";
    }

    private IEnumerator ChangePriceSequence(float score)
    {
        float fromScore = m_currentScore;
        float toScore = m_gameManager.CurrentScore;
        yield return CoroutineUtils.LerpWithTime(m_scoreChangeDuration, t =>
        {
            m_currentScore = Mathf.Lerp(fromScore, toScore, t);
            SetPriceText(m_currentScore);
        });
        SetPriceText(toScore);
        m_changePriceCoroutine = null;
    }

    private void SetPriceText(float score)
    {
        m_text.text = $"{score:F2}";
    }
}
