using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using UnityEngine;

public class UICartDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private CartItems m_cartItems;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Transform m_entryContainer;
    [SerializeField] private GameObject m_entryPrefab;
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;
    
    private ManagedObjectPool<UICartDisplay, UICartEntry> m_uiCartEntryPool;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_uiCartEntryPool = new ManagedObjectPool<UICartDisplay, UICartEntry>(this, m_entryContainer, m_entryPrefab);
        m_entryPrefab.SetActive(false);
        
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_cartItems.ItemsChanged += OnCartItemsChanged;
        OnCartItemsChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }

        if (m_cartItems != null)
        {
            m_cartItems.ItemsChanged -= OnCartItemsChanged;
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

    private void OnCartItemsChanged()
    {
        PopulateEntries(m_cartItems.Items);
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }

    private void PopulateEntries(List<Item> items)
    {
        m_uiCartEntryPool.DespawnAll();
        
        foreach (Item item in items)
        {
            UICartEntry entry = m_uiCartEntryPool.Spawn();
            entry.Setup(item.Icon);
        }
    }
}
