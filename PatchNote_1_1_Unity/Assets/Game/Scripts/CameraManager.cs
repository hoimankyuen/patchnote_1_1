using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain m_cinemachineBrain;
    [Space(5)]
    [Header("Components (Map Preview)")]
    [SerializeField] private GameObject m_previewVCamSetup;
    [SerializeField] private CinemachineSplineDolly m_splineDolly;
    
    [Header("Components (Trolley)")]
    [SerializeField] private GameObject m_trolleyVCamSetup;


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
}
