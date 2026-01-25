using System.Collections;
using System.Collections.Generic;
using MoonlightTools.GeneralTools;
using TMPro;
using UnityEngine;

public class UIAnnouncementDisplay : MonoBehaviour
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
    
    private Coroutine m_showTextCoroutine;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        m_gameManager.CurrentLapChanged += OnCurrentLapChanged;
    }
    
    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
            m_gameManager.CurrentLapChanged -= OnCurrentLapChanged;
        }
    }
    
    // ======== Other Events ========
    
    private void OnCurrentStateChanged()
    {
        if (m_gameManager.CurrentState == GameManager.State.Countdown)
        {
            m_canvasGroup.alpha = 0;
            AnnounceCountdown();
        }
        else
        {
            m_canvasGroup.alpha = 0;
        }
    }

    private void OnCurrentLapChanged()
    {
        if (m_gameManager.CurrentState is not GameManager.State.Playing and not GameManager.State.Paused)
            return;

        if (m_gameManager.CurrentLap > 0)
        {
            AnnounceText($"LAP {m_gameManager.CurrentLap + 1}");
        }
        else if (m_gameManager.CurrentLap == LevelManager.Instance.CurrentLevelData.Laps.Count - 1)
        {
            AnnounceText($"FINAL LAP");
        }
    }
    
    // ======== Functionalities ========

    private void AnnounceCountdown()
    {
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        yield return new WaitForSeconds (m_countdownWaitDuration);
        
        for (int i = m_countFrom; i >= 1; i--)
        {
            AnnounceText(i.ToString());
            yield return new WaitForSeconds(m_countdownTickDuration);
        }
        
        m_gameManager.CompleteCountdown();
        AnnounceText("GO!");
    }

    private void AnnounceText(string text)
    {
        if (m_showTextCoroutine != null)
        {
            StopCoroutine(m_showTextCoroutine);
            m_showTextCoroutine = null;
        }
        m_showTextCoroutine = StartCoroutine(ShowTextSequence(text));
    }
    
    private IEnumerator ShowTextSequence(string text)
    {
        SetText(text);
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
