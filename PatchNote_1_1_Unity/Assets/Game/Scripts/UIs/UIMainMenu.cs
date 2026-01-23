using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    public enum Page
    {
        Main,
        Settings,
        Levels,
        Credits
    }

    [Header("Components (Main Page)")]
    [SerializeField] private GameObject m_mainPage;
    [SerializeField] private Button m_mainPageFirstButton;

    // ======== Unity Events ========

    private void Start()
    {
        SwitchPage(Page.Main);
    }

    // ======== General Functionalities ========

    private void SwitchPage(Page page)
    {
        switch (page)
        {
            case Page.Main:
                ShowMainPage();
                break;
            default:
                break;
        }
    }

    private void HideAllPages()
    {
        m_mainPage.SetActive(false);
    }
    
    // ======== Main Page ========

    private void ShowMainPage()
    {
        m_mainPage.SetActive(true);
        m_mainPageFirstButton.Select();
    }

    public void MainPageStart()
    {
        LevelManager.Instance.GotoLevel(0);
    }

    public void MainPageSettings()
    {
        
    }

    public void MainPageCredits()
    {
        
    }
}
