using System;
using MoonlightTools.AudioSystem;
using MoonlightTools.FPSSystem;
using MoonlightTools.StoredDataSystem;
using MoonlightTools.UIComponents;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsMenu : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Panel m_panel;
    [SerializeField] private Slider m_musicSlider;
    [SerializeField] private Slider m_sfxSlider;
    [SerializeField] private Slider m_sensitivitySlider;
    [SerializeField] private Toggle m_vInvertToggle;
    [SerializeField] private Slider m_fovSlider;
    [SerializeField] private Toggle m_fpsToggle;
    
    private event Action OnClose;
    
    // ================ Functionalities ================

    public void Setup(Action onClose)
    {
        Populate();
        OnClose = onClose;
    }
    
    private void Populate()
    {
        m_musicSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume * 4f);
        m_sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SoundVolume * 4f);
        m_sensitivitySlider.SetValueWithoutNotify(SettingsManager.Instance.Sensitivity * 4f);
        m_vInvertToggle.SetIsOnWithoutNotify(SettingsManager.Instance.VerticalInvert);
        m_fovSlider.SetValueWithoutNotify(SettingsManager.Instance.Sensitivity * 4f);
        m_fpsToggle.SetIsOnWithoutNotify(FPSManager.Instance.ShowFPS);
    }

    public void OnMusicValueChanged(float value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        AudioManager.Instance.SetMusicVolume(value / 4f);   
    }

    public void OnSFXValueChanged(float value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        AudioManager.Instance.SetSoundVolume(value / 4f); 
    }

    public void OnSensitivityChanged(float value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        SettingsManager.Instance.SetSensitivity(value / 4f);
    }
    
    public void OnVInvertValueChanged(bool value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        SettingsManager.Instance.SetVerticalInvert(value);
    }

    public void OnFOVValueChanged(float value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        SettingsManager.Instance.SetFOV(value / 4f);
    }

    public void OnFPSValueChanged(bool value)
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        FPSManager.Instance.SetShowFPS(value);
    }
    
    public void Show()
    {
        m_panel.Show(true);
        m_musicSlider.Select();
    }
    
    public void Back()
    {
        AudioManager.Instance.PlaySoundEffect("Button_press01");
        StoredDataManager.Instance.Save(DataCategory.Settings);
        m_panel.Show(false);
        OnClose?.Invoke();
    }
}
