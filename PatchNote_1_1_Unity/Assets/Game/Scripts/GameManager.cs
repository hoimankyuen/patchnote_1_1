using System;
using Input;
using MoonlightTools.GeneralTools;
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

    [Header("References")] 
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private CameraManager m_cameraManager;
    [SerializeField] private Timer m_levelTimer;

    public State CurrentState { get; private set; }
    public event Action CurrentStateChanged;
    
    public int CurrentLap { get; private set; }
    public event Action CurrentLapChanged;
    
    public float CurrentScore { get; private set; }
    public event Action CurrentScoreChanged;
    
    public Timer LevelTimer => m_levelTimer;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_inputReader.Pause += Pause;
        m_levelTimer.Completed += LevelTimeOut;
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

        if (m_levelTimer != null)
        {
            m_levelTimer.Completed -= LevelTimeOut;
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
        
        CurrentLap = 0;
        CurrentLapChanged?.Invoke();

        CurrentScore = 0;
        CurrentScoreChanged?.Invoke();
        
        m_levelTimer.Setup(LevelManager.Instance.CurrentLevelData.timeLimit);
        m_levelTimer.StartTimer();
    }
    
    public void Restart()
    {
        LevelManager.Instance.GotoLevel(LevelManager.Instance.CurrentLevelData.number);
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

    private void LevelTimeOut()
    {
        if (CurrentState is not State.Playing and not State.Paused)
            return;
    }
}
