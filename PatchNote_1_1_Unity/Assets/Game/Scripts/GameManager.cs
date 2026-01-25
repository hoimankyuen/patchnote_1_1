using System;
using System.Collections.Generic;
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

    public State CurrentState { get; private set; } = State.None;
    public event Action CurrentStateChanged;

    public int CurrentLap { get; private set; } = -1;
    public event Action CurrentLapChanged;
    
    public List<RequirementData> CurrentRequirements { get; private set; } = new List<RequirementData>();
    public event Action CurrentRequirementsChanged;
    
    public float CurrentScore { get; private set; }
    public event Action CurrentScoreChanged;
    
    public bool JamFound { get; private set; }
    public event Action JamFoundChanged;
    
    public bool GemFound { get; private set; }
    public event Action GemFoundChanged;
    
    public Timer LevelTimer => m_levelTimer;
    
    // ======== Unity Events ========
    
    public void Awake()
    {
        m_inputReader.Pause += Pause;
        m_levelTimer.Completed += LevelTimeOut;
    }
    
    private void Start()
    {
        StartMapPreviewState();
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

    private void StartMapPreviewState()
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

        StartCountdownState();
    }
    
    // ======== Countdown ========

    private void StartCountdownState()
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
        
        StartPlayingState();
    }

    // ======== Playing and Paused ======== 
    
    private void StartPlayingState()
    {
        CurrentState = State.Playing;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.EnablePlayerInput();
        m_inputReader.DisableUIInput();
        Cursor.lockState = CursorLockMode.Locked;
        
        CurrentScore = 0;
        CurrentScoreChanged?.Invoke();
        
        JamFound = false;
        JamFoundChanged?.Invoke();
        
        GemFound = false;
        GemFoundChanged?.Invoke();
        
        m_levelTimer.Setup(LevelManager.Instance.CurrentLevelData.timeLimit);
        m_levelTimer.StartTimer();

        ProgressToNextLap();
    }

    private void ProgressToNextLap()
    {
        CurrentLap++;
        CurrentRequirements.Clear();

        if (CurrentLap < LevelManager.Instance.CurrentLevelData.laps.Count)
        {
            foreach (RequirementData requirement in
                     LevelManager.Instance.CurrentLevelData.laps[CurrentLap].requirements)
            {
                CurrentRequirements.Add(new RequirementData(requirement));
            }
            CurrentLapChanged?.Invoke();
        }
        else
        {
            StartEndedState();
        }
    }

    public void SolidifyProgress(List<Item> items, float score)
    {
        foreach (Item item in items)
        {
            RequirementData requirementData = CurrentRequirements.Find(x => x.itemType == item.ItemType);
            if (requirementData != null)
            {
                Debug.Log("Solidified" + requirementData.itemType);
                requirementData.quantity--;
            }

            if (!JamFound && item.ItemType == ItemType.Jam)
            {
                JamFound = true;
                JamFoundChanged?.Invoke();
            }

            if (!GemFound && item.ItemType == ItemType.Gem)
            {
                GemFound = true;
                GemFoundChanged?.Invoke();
            }
        }
        CurrentRequirementsChanged?.Invoke();
        
        CurrentScore += score;
        CurrentScoreChanged?.Invoke();

        // check for lap completion
        if (CurrentRequirements.TrueForAll(x => x.quantity <= 0))
        {
            ProgressToNextLap();
        }
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

        StartEndedState();
    }
    
    // ======== Ended ========

    private void StartEndedState()
    {
        CurrentState = State.Ended;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;
    }
}
