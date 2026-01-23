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
    public event Action CurrentStateChanged;

    public void Awake()
    {
        m_inputReader.Pause += Pause;
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
            
            m_inputReader.DisablePlayerInput();
            m_inputReader.DisableUIInput();
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    // ======== Map Preview ========

    private void StartMapPreview()
    {
        CurrentState = State.MapPreview;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        m_cameraManager.StartMapPreviewCamera();
        
        Time.timeScale = 1f;
    }

    public void CompleteMapPreview()
    {
        if (CurrentState != State.MapPreview)
            return;

        StartCountdown();
    }
    
    // ======== Countdown ========

    private void StartCountdown()
    {
        CurrentState = State.Countdown;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.DisableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        m_cameraManager.StartTrolleyCamera();
    }

    public void CompleteCountdown()
    {
        if (CurrentState != State.Countdown)
            return;
        
        StartPlaying();
    }

    // ======== Playing and Paused ======== 

    private void StartPlaying()
    {
        CurrentState = State.Playing;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.EnablePlayerInput();
        m_inputReader.DisableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void Restart()
    {
        LevelManager.Instance.GotoLevel(LevelManager.Instance.CurrentLevelInfo.number);
    }
    
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

    public void BackToManuMenu()
    {
        LevelManager.Instance.GotoMainMenu();
    }
}
