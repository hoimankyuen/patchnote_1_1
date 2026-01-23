using System.Collections.Generic;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;

public class UISpeedDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private Cart m_cart;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private List<RectTransform> m_dials;

    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;

    private float m_currentSpeed = 0f;
    private float m_targetSpeed = 0f;
    
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_cart.CurrentSpeedChanged += OnCurrentSpeedChanged;
        OnCurrentSpeedChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }

        if (m_cart != null)
        {
            m_cart.CurrentSpeedChanged -= OnCurrentSpeedChanged;
        }
    }

    private void Update()
    {
        ShowHide();
        ShowSpeed();
    }
    
    // ======== Other Events ========

    private void OnCurrentStateChanged()
    {
        m_targetAlpha = m_gameManager.CurrentState
            is GameManager.State.Playing
            or GameManager.State.Paused 
            ? 1f : 0f;
    }

    private void OnCurrentSpeedChanged()
    {
        m_targetSpeed = m_cart.CurrentSpeed;
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }

    private void ShowSpeed()
    {
        if (Mathfx.LinearMatch(m_targetSpeed, ref m_currentSpeed,  m_cart.MaxSpeed, 0.5f, Time.unscaledDeltaTime))
            return;
        
        m_text.text = $"{(m_currentSpeed * 2.23694f):F1} MPH";

        float percentage = m_currentSpeed / m_cart.MaxSpeed;
        foreach (RectTransform dial in m_dials)
        {
            dial.localEulerAngles = dial.localEulerAngles.ReplaceZ(Mathf.Lerp(180f, -90, percentage));
        }
    }
}
