using Input;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using TMPro;
using UnityEngine;

public class UIMapPreviews : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private InputReader m_inputReader;

    [Header("Components")]
    [SerializeField] private GameObject m_contents;
    [SerializeField] private TextMeshProUGUI m_levelNameText;
    [SerializeField] private CanvasGroup m_continueCanvasGroup;

    [Header("Settings")] 
    [SerializeField] private float m_durationBeforeContinue;
    [SerializeField] private float m_continueTextBlinkDuration;

    private float m_ElapseTime;
    
    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        m_inputReader.Continue += OnSubmitInput;
    }

    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }

        if (m_inputReader != null)
        {
            m_inputReader.Continue -= OnSubmitInput;
        }
    }
    
    private void Update()
    {
        BlinkContinueText();
    }
    
    private void OnCurrentStateChanged()
    {
        if (m_gameManager.CurrentState == GameManager.State.MapPreview)
        {
            m_contents.SetActive(true);
            m_ElapseTime = 0;
        }
        else
        {
            m_contents.SetActive(false);
        }
    }
    
    private void OnSubmitInput()
    {
        if (m_gameManager.CurrentState != GameManager.State.MapPreview)
            return;

        if (m_ElapseTime < m_durationBeforeContinue)
            return;
        
        m_gameManager.CompleteMapPreview();
    }

    private void BlinkContinueText()
    {
        if (m_gameManager.CurrentState != GameManager.State.MapPreview)
            return;
        
        m_ElapseTime += Time.unscaledDeltaTime;

        if (m_ElapseTime >= m_durationBeforeContinue)
        {
            float elapseTime = (m_ElapseTime - m_durationBeforeContinue) % m_continueTextBlinkDuration;
            float halfDuration = m_continueTextBlinkDuration / 2f;
            m_continueCanvasGroup.alpha = elapseTime < halfDuration
                ? Mathfx.Coserp(1, 0, elapseTime / halfDuration)
                : Mathfx.Sinerp(0, 1, (elapseTime - halfDuration) / halfDuration);
        }
        else
        {
            m_continueCanvasGroup.alpha = 0;
        }
    }
}
