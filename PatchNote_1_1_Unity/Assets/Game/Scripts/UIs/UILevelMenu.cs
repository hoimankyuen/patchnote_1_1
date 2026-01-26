using System;
using System.Collections.Generic;
using MoonlightTools.UIComponents;
using UnityEngine;
using UnityEngine.UI;

public class UILevelMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelLibrary m_levelLibrary;
    
    [Header("Components")]
    [SerializeField] private Panel m_panel;
    [SerializeField] private Transform m_levelButtonContainer;
    [SerializeField] private GameObject m_levelButtonPrefab;
    [SerializeField] private Button m_backButton;
    
    private List<UILevelMenuButton> m_levelButtons;
    private event Action OnClose;
    
    // ======== Functionalities ========

    public void Setup(Action onClose)
    {
        Populate();
        OnClose = onClose;
    }
    
    private void Populate()
    {
        m_levelButtonPrefab.gameObject.SetActive(false);
        
        m_levelButtons = new List<UILevelMenuButton>(); 
        for (int i = 0; i < m_levelLibrary.GetLevelCount(); i++)
        {
            GameObject go = Instantiate(m_levelButtonPrefab, m_levelButtonContainer);
            m_levelButtons.Add(go.GetComponent<UILevelMenuButton>());
        }

        int unlockedLevel = LevelManager.Instance.UnlockedLevel;
        for (int i = 0; i < m_levelButtons.Count; i++)
        {
            LevelManager.Instance.LoadLevelResults(i, out float score, out float time, out bool gem, out bool jam, out bool gold);
            m_levelButtons[i].Setup(m_levelLibrary.GetLevelInfo(i), i <= unlockedLevel, score, time, gem, jam, gold);
            m_levelButtons[i].ConnectButtons(
                i > 0 ? m_levelButtons[i - 1].Button : null,
                i < m_levelButtons.Count - 1 ? m_levelButtons[i + 1].Button : m_backButton);
            m_levelButtons[i].gameObject.SetActive(true);
        }

        if (m_levelButtons.Count > 0)
        {
            Navigation navigation = m_backButton.navigation;
            navigation.selectOnUp = m_levelButtons[^1].Button;
            m_backButton.navigation = navigation;
        }
    }
    
    public void Show()
    {
        m_panel.Show(true);
            
        if (m_levelButtons != null && m_levelButtons.Count > 0)
        {
            m_levelButtons[0].Button.Select();
        }
        else
        {
            m_backButton.Select();
        }
    }

    public void Back()
    {
        m_panel.Show(false);
        OnClose?.Invoke();
    }
}
