using Unity.Cinemachine;
using UnityEngine;

public class MenuSceneManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera m_vCam;
    
    private void Start()
    {
        SettingsManager.Instance.FOVChanged += OnFOVChanged;
        OnFOVChanged();
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.FOVChanged -= OnFOVChanged;
        }
    }
    
    private void OnFOVChanged()
    {
        m_vCam.Lens.FieldOfView = Mathf.Lerp(40f, 80f, SettingsManager.Instance.FOV);
    }
}
