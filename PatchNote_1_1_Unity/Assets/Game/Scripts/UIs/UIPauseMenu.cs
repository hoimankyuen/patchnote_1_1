using Input;
using MoonlightTools.UIComponents;
using TMPro;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    [SerializeField] private InputReader m_inputReader;

    [Header("Components")]
    [SerializeField] private Panel m_panel;

    public void Awake()
    {
        m_gameManager.CurrentStateChanged += OnCurrentStateChanged;
        m_inputReader.Resume += OnResumeInput;
        m_panel.IsVisibleChanged += OnIsVisibleChanged;
    }

    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.CurrentStateChanged -= OnCurrentStateChanged;
        }

        if (m_inputReader != null)
        {
            m_inputReader.Resume -= OnResumeInput;
        }

        if (m_panel != null)
        {
            m_panel.IsVisibleChanged -= OnIsVisibleChanged;
        }
    }

    private void OnCurrentStateChanged()
    {
        if (m_gameManager.CurrentState == GameManager.State.Paused)
        {
            m_panel.Show(true);
        }
    }

    private void OnResumeInput()
    {
        if (m_gameManager.CurrentState != GameManager.State.Paused)
            return;
        
        m_panel.Show(false);
    }

    private void OnIsVisibleChanged()
    {
        if (m_gameManager.CurrentState != GameManager.State.Paused)
            return;

        if (m_panel.IsVisible)
            return;
        
        m_gameManager.Resume();
    }
}
