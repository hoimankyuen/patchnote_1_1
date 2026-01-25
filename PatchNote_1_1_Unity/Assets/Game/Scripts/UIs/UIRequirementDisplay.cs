using System.Collections;
using System.Collections.Generic;
using MoonlightTools.MathTools;
using UnityEngine;

public class UIRequirementDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
   
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Transform m_requirementEntryContainer;
    [SerializeField] private GameObject m_requirementEntryPrefab;

    [Header("settings")]
    [SerializeField] private float m_entryShowDuration;
    [SerializeField] private float m_entryShowDelay;
    
    private float m_currentAlpha = 0f;
    private float m_targetAlpha = 0f;

    private readonly List<UIRequirementEntry> m_requirementEntryInstances =  new List<UIRequirementEntry>();
     
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        OnCurrentStateChanged();
        
        m_gameManager.CurrentLapChanged += OnCurrentLapChanged;
        OnCurrentLapChanged();

        m_gameManager.CurrentRequirementsChanged += OnCurrentRequirementsChanged;
        OnCurrentRequirementsChanged();
        
        m_currentAlpha = m_canvasGroup.alpha;
        
        m_requirementEntryPrefab.SetActive(false);
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.LevelTimer.RemainingTimeChanged -= OnCurrentLapChanged;
            m_gameManager.CurrentRequirementsChanged -= OnCurrentRequirementsChanged;
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
        PopulateRequirementDisplay(m_gameManager.CurrentRequirements);
    }

    private void OnCurrentRequirementsChanged()
    {
        UpdateConfirmedRequirementDisplay(m_gameManager.CurrentRequirements);
    }
    
    // ======== Functionalities ========

    private void ShowHide()
    {
        if (Mathfx.LinearMatch(m_targetAlpha, ref m_currentAlpha, 1f, 0.25f, Time.unscaledDeltaTime))
            return;
        
        m_canvasGroup.alpha = m_currentAlpha;
    }

    private void PopulateRequirementDisplay(ItemQuantities requirements)
    {
        StartCoroutine(PopulateRequirementDisplaySequence(requirements));
    }

    private IEnumerator PopulateRequirementDisplaySequence(ItemQuantities requirements)
    {
        // hide existing
        if (m_requirementEntryInstances.Count > 0)
        {
            foreach (UIRequirementEntry instance in m_requirementEntryInstances)
            {
                instance.Hide();
            }
            yield return new WaitForSeconds(m_entryShowDuration + m_entryShowDelay);
        }
        m_requirementEntryInstances.Clear();
        
        // show new
        List<ItemQuantity> requirementList = requirements.ToList();
        for (int i = 0; i < requirementList.Count; i++)
        {
            ItemQuantity requirementData = requirementList[i];
            
            GameObject go = Instantiate(m_requirementEntryPrefab, m_canvasGroup.transform);
            UIRequirementEntry entry = go.GetComponent<UIRequirementEntry>();
            go.SetActive(true);
            entry.Setup(i, requirementData.Type, requirementData.Quantity, m_entryShowDuration);
            entry.Show();
            m_requirementEntryInstances.Add(entry);
        }
    }

    private void UpdateConfirmedRequirementDisplay(ItemQuantities remaining)
    {
        foreach (UIRequirementEntry instance in m_requirementEntryInstances)
        {
            instance.SetCurrent(remaining.GetQuantity(instance.ItemType));
        }
    }
}
