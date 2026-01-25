using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private UILevelMenu m_levelMenu;
    [SerializeField] private UICreditMenu m_creditMenu;
    
    [Header("Components")]
    [SerializeField] private GameObject m_mainPage;
    [SerializeField] private Button m_mainPageFirstButton;

    
    // ======== Unity Events ========

    private void Start()
    {
        m_levelMenu.Setup(SelectFirstButton);
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
        LevelManager.Instance.GotoLevel(0);
    }

    public void ShowLevelMenu()
    {
        m_levelMenu.Show();
    }
    
    public void ShowSettingsMenu()
    {
        
    }

    public void ShowCreditsMenu()
    {
        m_creditMenu.Show();
    }
}
