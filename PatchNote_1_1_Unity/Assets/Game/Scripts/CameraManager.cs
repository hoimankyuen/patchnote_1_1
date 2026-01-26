using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Components (General)")]
    [SerializeField] private CinemachineBrain m_cinemachineBrain;

    [Header("Components (Map Preview)")]
    [SerializeField] private GameObject m_previewVCamSetup;
    [SerializeField] private CinemachineSplineDolly m_splineDolly;
    
    [Header("Components (Trolley)")]
    [SerializeField] private GameObject m_trolleyVCamSetup;
    [SerializeField] private CinemachineCamera m_trolleyVCam;
    [SerializeField] private CinemachineInputAxisController m_trolleyInput;
    [SerializeField] private CinemachineBasicMultiChannelPerlin m_trolleyShake;

    [Header("Settings")]
    [SerializeField] private float m_trolleyShakeDuration;
    
    private Coroutine m_trolleyShakeCoroutine;
    
    private void Start()
    {
        SettingsManager.Instance.SensitivityChanged += OnSensitivityChanged;
        OnSensitivityChanged();

        SettingsManager.Instance.VerticalInvertChanged += OnVerticalInvertChanged;
        OnVerticalInvertChanged();
        
        SettingsManager.Instance.FOVChanged += OnFOVChanged;
        OnFOVChanged();
    }

    private void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SensitivityChanged -= OnSensitivityChanged;
            SettingsManager.Instance.VerticalInvertChanged -= OnVerticalInvertChanged;
            SettingsManager.Instance.FOVChanged -= OnFOVChanged;
        }
    }

    private void OnSensitivityChanged()
    {
        foreach (var c in m_trolleyInput.Controllers)
        {
            if (c.Name == "Look X (Pan)")
            {
                c.Input.Gain = Mathf.Lerp(3f, 7f, SettingsManager.Instance.Sensitivity);
            }
            if (c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = Mathf.Lerp(3f, 7f, SettingsManager.Instance.Sensitivity)
                               * (SettingsManager.Instance.VerticalInvert ? 1f : -1f);
            }
        }
    }
    
    private void OnVerticalInvertChanged()
    {
        foreach (var c in m_trolleyInput.Controllers)
        {
            if (c.Name == "Look Y (Tilt)")
            {
                c.Input.Gain = Mathf.Lerp(3f, 7f, SettingsManager.Instance.Sensitivity) 
                               * (SettingsManager.Instance.VerticalInvert ? 1f : -1f);
                break;
            }
        }
    }
    
    private void OnFOVChanged()
    {
        m_trolleyVCam.Lens.FieldOfView = Mathf.Lerp(40f, 80f, SettingsManager.Instance.FOV);
    }

    private void StopAllCameras()
    {
        m_previewVCamSetup.SetActive(false);
        m_trolleyVCamSetup.SetActive(false);
    }

    public void StartMapPreviewCamera()
    {
        StopAllCameras();
        m_previewVCamSetup.SetActive(true);
        
        m_splineDolly.CameraPosition = 0;
    }

    public void StartTrolleyCamera()
    {
        StopAllCameras();
        m_trolleyVCamSetup.SetActive(true);
    }

    public void ShakeTrolleyCamera()
    {
        if (m_trolleyShakeCoroutine != null)
        {
            StopCoroutine(m_trolleyShakeCoroutine);
            m_trolleyShakeCoroutine = null;
        }
        m_trolleyShakeCoroutine = StartCoroutine(ShakeTrolleyCameraSequence());
    }

    private IEnumerator ShakeTrolleyCameraSequence()
    {
        m_trolleyShake.AmplitudeGain = 2f;
        yield return new WaitForSeconds(m_trolleyShakeDuration);
        m_trolleyShake.AmplitudeGain = 0f;
    }
}
