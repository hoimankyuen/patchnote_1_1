using System;
using Input;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        None,
        MapPreview,
        Countdown,
        Playing,
        Paused,
        Ended
    }

    [Header("References")] [SerializeField]
    private InputReader m_inputReader;

    [SerializeField] private CameraManager m_cameraManager;
    [SerializeField] private Cart m_cart;

    public State CurrentState { get; private set; }
    public Action CurrentStateChanged;

    public void Awake()
    {
        m_inputReader.Pause += Pause;
        m_inputReader.Resume += Resume;
    }
    
    private void Start()
    {
        StartMapPreview();
    }

    private void OnDestroy()
    {
        if (m_inputReader != null)
        {
            m_inputReader.Pause -= Pause;
            m_inputReader.Resume -= Resume;
            
            m_inputReader.DisablePlayerInput();
            m_inputReader.DisableUIInput();
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    // ======== Map Preview ========

    private void StartMapPreview()
    {
        if (CurrentState != State.None)
            return;
        
        CurrentState = State.MapPreview;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        m_cameraManager.StartMapPreviewCamera();
    }

    public void CompleteMapPreview()
    {
        if (CurrentState != State.MapPreview)
            return;
        
        CurrentState = State.Playing;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.EnablePlayerInput();
        m_inputReader.DisableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        m_cameraManager.StartTrolleyCamera();
    }
    
    // ======== Playing and Paused ======== 
    
    private void Pause()
    {
        if (CurrentState != State.Playing)
            return;
        
        CurrentState = State.Paused;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (CurrentState != State.Paused)
            return;
        
        CurrentState = State.Playing;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.EnablePlayerInput();
        m_inputReader.DisableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        Time.timeScale = 1f;
    }
}
