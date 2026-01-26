using System;
using System.Collections;
using System.Collections.Generic;
using MoonlightTools.AudioSystem;
using MoonlightTools.UIComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class UICreditMenu : MonoBehaviour
{
    [Serializable]
    public class EntryGroup
    {
        public string subtitle;
        public List<string> entries;
    }
    
    [Header("Components")]
    [SerializeField] private Panel m_panel;
    [SerializeField] private Transform m_container;
    [SerializeField] private GameObject m_subtitlePrefab;
    [SerializeField] private GameObject m_entryPrefab;
    [SerializeField] private GameObject m_spacerPrefab;
    [SerializeField] private ScrollRect m_scrollRect;
    [SerializeField] private Scrollbar m_scrollbar;
    [SerializeField] private Button m_backButton;
    
    [Header("Settings")]
    [SerializeField] private List<EntryGroup> m_entryGroups;
    
    private event Action OnClose;

    // ================ Functionalities ================

    public void Setup(Action onClose)
    {
        Populate();
        OnClose = onClose;
    }
    
    private void Populate()
    {
        foreach (EntryGroup entryGroup in m_entryGroups)
        {
            GameObject subtitleGO = Instantiate(m_subtitlePrefab, m_container);
            subtitleGO.GetComponentInChildren<TextMeshProUGUI>().text = entryGroup.subtitle;
            subtitleGO.SetActive(true);
           
            foreach (string entry in entryGroup.entries)
            {
                GameObject entryGO = Instantiate(m_entryPrefab, m_container);
                entryGO.GetComponentInChildren<TextMeshProUGUI>().text = entry;
                entryGO.SetActive(true);
            }
            GameObject spacerGO = Instantiate(m_spacerPrefab, m_container);
            spacerGO.SetActive(true);
        }
    }

    public void Show()
    {
        m_panel.Show(true);
        
        m_scrollbar.Select();
        StartCoroutine(WaitAndReposition());
    }

    private IEnumerator WaitAndReposition()
    {
        yield return new WaitForSeconds(0.25f);
        m_scrollRect.verticalNormalizedPosition = 1f;
        m_scrollbar.Select();
    }
    
    public void OnScrollValueChanged(Vector2 value)
    {
        if (value.y <= 0f)
        {
            m_backButton.Select();
            m_scrollRect.verticalNormalizedPosition = 0.01f;
        }
    }

    public void Back()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        m_panel.Show(false);
        OnClose?.Invoke();
    }
}