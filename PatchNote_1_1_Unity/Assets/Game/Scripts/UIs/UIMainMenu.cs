using MoonlightTools.AudioSystem;
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
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        LevelManager.Instance.GotoLevel(0);
    }

    public void MainPageSettings()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
    }

    public void MainPageCredits()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
    }
}
