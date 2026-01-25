using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.UI;

public class UIBonusDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Image m_gemFrameImage;
    [SerializeField] private Image m_gemFoundImage;
    [SerializeField] private Image m_jamFrameImage;
    [SerializeField] private Image m_jamFoundImage;
    [SerializeField] private Image m_goldFrameImage;
    [SerializeField] private Image m_goldFoundImage;
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;
    
    private ManagedObjectPool<UICartDisplay, UICartEntry> m_uiCartEntryPool;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnGemFoundChanged();
        
        m_gameManager.GemFoundChanged += OnGemFoundChanged;
        OnGemFoundChanged();
        
        m_gameManager.JamFoundChanged += OnJamFoundChanged;
        OnJamFoundChanged();
        
        m_gameManager.GoldFoundChanged += OnGoldFoundChanged;
        OnGoldFoundChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.GemFoundChanged -= OnGemFoundChanged;
            m_gameManager.JamFoundChanged -= OnJamFoundChanged;
            m_gameManager.GoldFoundChanged -= OnGoldFoundChanged;
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
 
    
    private void OnGemFoundChanged()
    {
        m_gemFoundImage.enabled = m_gameManager.GemFound;
    }

    private void OnJamFoundChanged()
    {
        m_jamFoundImage.enabled = m_gameManager.JamFound;
    }
    
    private void OnGoldFoundChanged()
    {
        m_goldFoundImage.enabled = m_gameManager.GoldFound;
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }
}
