using System;
using System.Collections;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimeDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_frame;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private CanvasGroup m_addCanvasGroup;
    [SerializeField] private TextMeshProUGUI m_addText;

    [Header("Settings")] 
    [SerializeField] private float m_alarmStartingTime;
    [SerializeField] private float m_alarmInterval;
    [SerializeField] private Color m_normalTextColour;
    [SerializeField] private Color m_alarmTextColour;

    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;
    
    private float m_lastTime = 0f;

    private Coroutine m_textAlarmCoroutine;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_gameManager.RemainingTimeChanged += OnRemainingTimeChanged;
        OnRemainingTimeChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.RemainingTimeChanged -= OnRemainingTimeChanged;
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

    private void OnRemainingTimeChanged()
    {
        float remainingTime = m_gameManager.RemainingTime;
        if (remainingTime > m_lastTime)
        {
            StartCoroutine(AddTimeSequence(remainingTime - m_lastTime));
        }
        m_lastTime = remainingTime;
        
        ShowTimeText(remainingTime);
        HandleTextColour(remainingTime);
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }

    private void ShowTimeText(float remainingTime)
    {
        m_text.text = $"{(int)(remainingTime/60):D2}:{(int)(remainingTime%60):D2}:{(int)(remainingTime*100%100):D2}";
    }

    private void HandleTextColour(float remainingTime)
    {
        if (remainingTime <= 0)
        {
            m_text.color = m_alarmTextColour;
            if (m_textAlarmCoroutine == null) 
                return;
            StopCoroutine(m_textAlarmCoroutine);
            m_textAlarmCoroutine = null;
        }
        else if (remainingTime <= m_alarmStartingTime)
        {
            if (m_textAlarmCoroutine != null)
                return;
            m_textAlarmCoroutine = StartCoroutine(TextAlarmSequence());
        }
        else
        {
            m_text.color = m_normalTextColour;
            
            if (m_textAlarmCoroutine == null)
                return;
            StopCoroutine(m_textAlarmCoroutine);
            m_textAlarmCoroutine = null;
        }
    }

    private IEnumerator TextAlarmSequence()
    {
        while (true)
        {
            m_text.color = m_alarmTextColour;
            yield return new WaitForSeconds(m_alarmInterval / 2f);
            m_text.color = m_normalTextColour;
            yield return new WaitForSeconds(m_alarmInterval / 2f);
        }
    }

    private IEnumerator AddTimeSequence(float addedTime)
    {
        m_addText.text = $"+{(int)(addedTime/60):D2}:{(int)(addedTime%60):D2}:{(int)(addedTime*100%100):D2}";
        m_addCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);
        yield return CoroutineUtils.LerpWithTime(2f, t =>
        {
            m_addCanvasGroup.alpha = Mathfx.Coserp(1f, 0f, t);
        });
        m_addCanvasGroup.alpha = 0f;
    }
}
