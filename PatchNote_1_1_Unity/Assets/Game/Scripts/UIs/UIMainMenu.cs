using MoonlightTools.AudioSystem;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private UILevelMenu m_levelMenu;
    [SerializeField] private UISettingsMenu m_settingsMenu;
    [SerializeField] private UICreditMenu m_creditMenu;
    
    [Header("Components")]
    [SerializeField] private GameObject m_mainPage;
    [SerializeField] private Button m_mainPageFirstButton;

    
    // ======== Unity Events ========

    private void Start()
    {
        m_levelMenu.Setup(SelectFirstButton);
        m_settingsMenu.Setup(SelectFirstButton);
        m_creditMenu.Setup(SelectFirstButton);
        
        SelectFirstButton();
    }

    // ======== Functionalities ========

    private void SelectFirstButton()
    {
        m_mainPageFirstButton.Select();
    }
    
    public void StartGame()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        LevelManager.Instance.GotoLevel(0);
    }

    public void ShowLevelMenu()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        m_levelMenu.Show();
    }
    
    public void ShowSettingsMenu()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        m_settingsMenu.Show();
    }

    public void ShowCreditsMenu()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        m_creditMenu.Show();
    }
}
