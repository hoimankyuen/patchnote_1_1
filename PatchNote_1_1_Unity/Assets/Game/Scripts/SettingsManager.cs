using System;
using MoonlightTools.GeneralTools;
using MoonlightTools.StoredDataSystem;
using UnityEngine;

public class SettingsManager : MonoBehaviourPersistentSingleton<SettingsManager>
{
    private const string SensitivityKey = "Sensitivity";
    private const string VerticalInvertKey = "VerticalInvert";
    private const string FOVKey = "FOV";

    public float Sensitivity { get; private set; } = 0.5f;
    public event Action SensitivityChanged;

    public bool VerticalInvert { get; private set; } = false;
    public event Action VerticalInvertChanged;
    
    public float FOV { get; private set; } = 0.5f;
    public event Action FOVChanged;
    
    // ================ Unity Messages ================

    private void Start()
    {
        InitSettings();
    }
    
    // ================ Functionalities ================

    private void InitSettings()
    {
        Sensitivity = StoredDataManager.Instance.GetOrSetupFloat(DataCategory.Settings, SensitivityKey, 0.5f);
        SensitivityChanged?.Invoke();
        
        VerticalInvert = StoredDataManager.Instance.GetOrSetupInt(DataCategory.Settings, VerticalInvertKey, 0) == 1;
        SensitivityChanged?.Invoke();
       
        FOV = StoredDataManager.Instance.GetOrSetupFloat(DataCategory.Settings, FOVKey, 0.5f);
        FOVChanged?.Invoke();
    }

    public void SetSensitivity(float value)
    {
        if (Mathf.Approximately(Sensitivity, value))
            return;
            
        StoredDataManager.Instance.SetFloat(DataCategory.Settings, SensitivityKey, value);
        Sensitivity = value;
        SensitivityChanged?.Invoke();
    }
    
    public void SetVerticalInvert(bool value)
    {
        if (VerticalInvert == value)
            return;
            
        StoredDataManager.Instance.SetFloat(DataCategory.Settings, VerticalInvertKey, value ? 1 : 0);
        VerticalInvert = value;
        VerticalInvertChanged?.Invoke();
    }
    
    public void SetFOV(float value)
    {
        if (Mathf.Approximately(FOV, value))
            return;
            
        StoredDataManager.Instance.SetFloat(DataCategory.Settings, FOVKey, value);
        FOV = value;
        FOVChanged?.Invoke();
    }
}
