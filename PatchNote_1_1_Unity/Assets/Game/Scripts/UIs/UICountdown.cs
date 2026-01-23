using System.Collections;
using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class UICountdown : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
  
    [Header("Components")]
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private List<TextMeshProUGUI> m_texts;

    [Header("Settings")] 
    [SerializeField] private int m_countFrom = 3;
    [SerializeField] private float m_countdownWaitDuration;
    [SerializeField] private float m_countdownTickDuration;
    [SerializeField] private float m_textStayDuration;
    [SerializeField] private float m_textDecayDuration;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }
    }
    
    // ======== Other Events ========
    
    private void OnCurrentStateChanged()
    {
        if (m_gameManager.CurrentState == GameManager.State.Countdown)
        {
            m_canvasGroup.alpha = 0;
            StartCoroutine(CountdownSequence());
        }
        else
        {
            m_canvasGroup.alpha = 0;
        }
    }

    private IEnumerator CountdownSequence()
    {
        yield return new WaitForSeconds (m_countdownWaitDuration);

        // numbers
        for (int i = m_countFrom; i >= 1; i--)
        {
            SetText(i.ToString());
            yield return new WaitForSeconds(Mathf.Min(m_countdownTickDuration, m_textStayDuration));
            yield return CoroutineUtils.LerpWithTime(
                Mathf.Max(Mathf.Min(m_countdownTickDuration - m_textStayDuration, m_textDecayDuration), 0),
                t =>
                {
                    m_canvasGroup.alpha = 1 - t;
                });
            yield return new WaitForSeconds(Mathf.Max(m_countdownTickDuration - m_textStayDuration - m_textDecayDuration, 0));
        }
        
        m_gameManager.CompleteCountdown();
        
        // go
        SetText("GO!");
        yield return new WaitForSeconds(m_textStayDuration);
        yield return CoroutineUtils.LerpWithTime(m_textDecayDuration,
            t =>
            {
                m_canvasGroup.alpha = 1 - t;
            });
    }

    private void SetText(string s)
    {
        foreach (TextMeshProUGUI text in m_texts)
        {
            text.text = s;
        }
        m_canvasGroup.alpha = 1;
    }
}
