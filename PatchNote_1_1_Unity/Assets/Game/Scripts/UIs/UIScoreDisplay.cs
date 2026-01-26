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
    [SerializeField] private CartItems m_cartItems;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_frame;
    [SerializeField] private TextMeshProUGUI m_confirmedScoreText;
    [SerializeField] private TextMeshProUGUI m_pendingScoreText;

    [Header("Settings")]
    [SerializeField] private float m_scoreChangeDuration;    
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;

    private float m_currentConfirmed = 0f;
    private float m_currentPending = 0f;
    private Coroutine m_changeConfirmedCoroutine;
    private Coroutine m_changePendingCoroutine;
    
    // ======== Unity Events ========
    
    public void Start()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_gameManager.CurrentScoreChanged += OnCurrentScoreChanged;
        OnCurrentScoreChanged();
        
        m_cartItems.TotalScoreChanged += OnCartTotalScoreChanged;
        OnCartTotalScoreChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.CurrentScoreChanged -= OnCurrentScoreChanged;
        }

        if (m_cartItems != null)
        {
            m_cartItems.TotalScoreChanged -= OnCartTotalScoreChanged;
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
        ChangeConfirmed(m_gameManager.CurrentScore);
    }
    
    private void OnCartTotalScoreChanged()
    {
        ChangePending(m_cartItems.TotalScore);
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }
    
    private void ChangeConfirmed(float score)
    {
        if (m_changeConfirmedCoroutine != null)
        {
            StopCoroutine(m_changeConfirmedCoroutine);
            m_changeConfirmedCoroutine = null;
        }
        m_changeConfirmedCoroutine = StartCoroutine(ChangeConfirmedSequence(score));
    }

    private IEnumerator ChangeConfirmedSequence(float score)
    {
        float fromScore = m_currentConfirmed;
        float toScore = m_gameManager.CurrentScore;
        yield return CoroutineUtils.LerpWithTime(m_scoreChangeDuration, t =>
        {
            m_currentConfirmed = Mathf.Lerp(fromScore, toScore, t);
            SetConfirmedText(m_currentConfirmed);
        });
        SetConfirmedText(toScore);
        m_changeConfirmedCoroutine = null; 
    }

    private void SetConfirmedText(float score)
    {
        m_confirmedScoreText.text = $"{(int)Mathf.Min(score, 9999)} + (   )";
    }
    
    private void ChangePending(float score)
    {
        if (m_changePendingCoroutine != null)
        {
            StopCoroutine(m_changePendingCoroutine);
            m_changePendingCoroutine = null;
        }
        m_changePendingCoroutine = StartCoroutine(ChangePendingSequence(score));
    }
    
    private IEnumerator ChangePendingSequence(float score)
    {
        float fromScore = m_currentPending;
        float toScore = m_cartItems.TotalScore;
        yield return CoroutineUtils.LerpWithTime(m_scoreChangeDuration, t =>
        {
            m_currentPending = Mathf.Lerp(fromScore, toScore, t);
            SetPendingText(m_currentPending);
        });
        SetPendingText(toScore);
        m_changePendingCoroutine = null; 
    }
    
    private void SetPendingText(float score)
    {
        m_pendingScoreText.text = $"{(int)Mathf.Min(score, 999)})";
    }
}
