using MoonlightTools.AudioSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelMenuButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button m_button;
    [SerializeField] private TextMeshProUGUI m_nameText;
    [SerializeField] private TextMeshProUGUI m_scoreText;
    [SerializeField] private TextMeshProUGUI m_timeText;
    [SerializeField] private Image m_gemImage;
    [SerializeField] private Image m_jamImage;
    [SerializeField] private Image m_goldImage;

    private LevelData m_levelData;

    public Button Button => m_button;
        
    public void Setup(LevelData levelData, bool unlocked, float score, float time, bool gem, bool jam, bool gold)
    {
        m_button.interactable = unlocked;
        m_levelData = levelData;
        m_nameText.text = $"{levelData.Number + 1}. {levelData.Title}";
        m_scoreText.text = $"$ {(int)score}";
        m_timeText.text = $"{(int)(time/60):D2}:{(int)(time%60):D2}:{(int)(time*100%100):D2}";
        m_gemImage.gameObject.SetActive(gem);
        m_jamImage.gameObject.SetActive(jam);
        m_goldImage.gameObject.SetActive(gold);
    }

    public void ConnectButtons(Button above, Button below)
    {
        Navigation navigation = m_button.navigation;
        navigation.selectOnUp = above;
        navigation.selectOnDown = below;
        m_button.navigation = navigation;
    }

    public void LoadLevel()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        LevelManager.Instance.GotoLevel(m_levelData.Number);
    }
}
