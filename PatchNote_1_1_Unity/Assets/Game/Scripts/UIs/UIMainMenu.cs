using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private UILevelMenu m_uiLevelMenu;
    
    [Header("Components")]
    [SerializeField] private GameObject m_mainPage;
    [SerializeField] private Button m_mainPageFirstButton;

    
    // ======== Unity Events ========

    private void Start()
    {
        m_uiLevelMenu.Setup(SelectFirstButton);
        
        SelectFirstButton();
    }

    // ======== Functionalities ========

    private void SelectFirstButton()
    {
        m_mainPageFirstButton.Select();
    }
    
    public void StartGame()
    {
        LevelManager.Instance.GotoLevel(0);
    }

    public void ShowLevelMenu()
    {
        m_uiLevelMenu.Show();
    }
    
    public void ShowSettingsMenu()
    {
        
    }

    public void ShowCreditsMenu()
    {
        
    }
}
