using System.Collections;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRequirementEntry : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemLibrary m_ItemLibrary;
    
    [Header("Components")]
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_tick;
    [SerializeField] private TextMeshProUGUI m_text;
    
    private int m_order; 
    private int m_requiredQuantity;
    private int m_currentQuantity;
    private float m_showHideDuration;
    
    public ItemType ItemType { get; private set; }
    
    // ======== Unity Messages ========

    private void Awake()
    {
        m_CanvasGroup.alpha = 0;
    }

    // ======== Functionalities ========
    
    public void Setup(int order, ItemType itemType, int requiredQuantity, float showHideDuration)
    {
        ItemType = itemType;
        m_order = order;
        m_icon.sprite = m_ItemLibrary.GetItemData(itemType).icon;
        m_requiredQuantity = requiredQuantity;
        m_currentQuantity = 0;
        m_showHideDuration = showHideDuration;
        UpdateDisplay();
    }

    public void SetAmount(int amount)
    {
        m_currentQuantity = amount;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {  
        bool fulfilled = m_currentQuantity >= m_requiredQuantity;
        m_tick.gameObject.SetActive(fulfilled);
        m_text.gameObject.SetActive(!fulfilled);
        m_text.text = $"{m_currentQuantity}/{m_requiredQuantity}";
    }

    public void Show()
    {
        StartCoroutine(ShowHideSequence(true));
    }

    public void Hide()
    {
        StartCoroutine(ShowHideSequence(false));
    }

    private IEnumerator ShowHideSequence(bool show)
    {
        if (show)
        {
            yield return new WaitForSecondsRealtime(m_showHideDuration * m_order / 2f);
        }
        yield return CoroutineUtils.LerpWithTime(m_showHideDuration, t =>
        {
            m_CanvasGroup.alpha = show ? Mathfx.Sinerp(0f, 1f, t) : Mathfx.Sinerp(1f, 0f, t);
        });
        m_CanvasGroup.alpha = show ? 1f : 0f;
        if (!show)
        {
            Destroy(gameObject);
        }
    }
}
