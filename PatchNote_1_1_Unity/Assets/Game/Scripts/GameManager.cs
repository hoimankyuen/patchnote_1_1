using System;
using System.Collections.Generic;
using Input;
using MoonlightTools.AudioSystem;
using MoonlightTools.GeneralTools;
using MoonlightTools.StoredDataSystem;
using UnityEngine;
using UnityEngine.Audio;

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
    [SerializeField] private AudioSource m_musicAudioSource;
    
    [SerializeField] private AudioResource m_gameMusic;
    [SerializeField] private AudioResource m_previewMusic;
    
    public State CurrentState { get; private set; } = State.None;
    public event Action CurrentStateChanged;

    public int CurrentLap { get; private set; } = -1;
    public event Action CurrentLapChanged;
    
    public float TotalTime { get; private set; }
    public float RemainingTime { get; private set; }
    public event Action RemainingTimeChanged;
    
    public ItemQuantities CurrentRequirements { get; private set; } = new ItemQuantities();
    public event Action CurrentRequirementsChanged;
    
    public int CurrentItemCount { get; private set; }
    public event Action CurrentItemCountChanged;
    
    public float CurrentScore { get; private set; }
    public event Action CurrentScoreChanged;
    
    public bool GemFound { get; private set; }
    public event Action GemFoundChanged;
    
    public bool JamFound { get; private set; }
    public event Action JamFoundChanged;
    
    public bool GoldFound { get; private set; }
    public event Action GoldFoundChanged;
    
    public bool LevelCompleted { get; private set; }
    
    public void Awake()
    {
        m_inputReader.Pause += Pause;
    }
    
    private void Start()
    {
        AudioManager.Instance.StopMusic();
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
    }

    private void Update()
    {
        HandleTime();
    }
    
    // ======== Map Preview ========

    private void StartMapPreviewState()
    {
        CurrentState = State.MapPreview;
        CurrentStateChanged?.Invoke();
        
        m_musicAudioSource.resource = m_previewMusic;
        m_musicAudioSource.Play();
        
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
        
        ProgressToNextLap();
        
        m_musicAudioSource.Stop();
        m_musicAudioSource.resource = m_gameMusic;
        m_musicAudioSource.Play();
    }

    private void HandleTime()
    {
        if (CurrentState is not State.Playing and not State.Paused)
            return;

        TotalTime += Time.deltaTime;
        RemainingTime -= Time.deltaTime;
        RemainingTimeChanged?.Invoke();
    }
    
    private void LevelTimeOut()
    {
        if (CurrentState is not State.Playing and not State.Paused)
            return;

        LevelCompleted = false;
        StartEndedState();
    }

    private void ProgressToNextLap()
    {
        CurrentLap++;
        CurrentRequirements.Clear();

        if (CurrentLap < LevelManager.Instance.CurrentLevelData.Laps.Count)
        {
            RemainingTime += LevelManager.Instance.CurrentLevelData.Laps[CurrentLap].TimeAllocated;
            RemainingTimeChanged?.Invoke();
            
            List<ItemType> selectedItemTypes = new List<ItemType>();
            foreach (ItemGroupQuantity groupQuantity in LevelManager.Instance.CurrentLevelData.Laps[CurrentLap].Requirements)
            {
                ItemQuantity itemQuantity = groupQuantity.ResolveToItemQuantity(selectedItemTypes);
                CurrentRequirements.Add(itemQuantity);
                selectedItemTypes.Add(itemQuantity.Type);
            }
            CurrentLapChanged?.Invoke();
        }
        else
        {
            LevelCompleted = true;
            StartEndedState();
        }
    }

    public void SolidifyProgress(List<Item> items, float score)
    {
        foreach (Item item in items)
        {
            CurrentRequirements.ChangeQuantityIfExists(item.Type, -1);

            if (!JamFound && item.Type == ItemType.Jam)
            {
                JamFound = true;
                JamFoundChanged?.Invoke();
            }

            if (!GemFound && item.Type == ItemType.Gem)
            {
                GemFound = true;
                GemFoundChanged?.Invoke();
            }
            
            if (!GoldFound && item.Type == ItemType.Gold)
            {
                GoldFound = true;
                GoldFoundChanged?.Invoke();
            }
        }
        CurrentRequirementsChanged?.Invoke();
        
        CurrentScore += score;
        CurrentScoreChanged?.Invoke();
        
        CurrentItemCount += items.Count;
        CurrentItemCountChanged?.Invoke();

        // check for lap completion
        if (CurrentRequirements.TrueForAll(x => x.Quantity <= 0))
        {
            ProgressToNextLap();
        }
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
    
    // ======== Ended ========

    private void StartEndedState()
    {
        CurrentState = State.Ended;
        CurrentStateChanged?.Invoke();
        
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;

        SaveLevelResults();
    }

    private void SaveLevelResults()
    {
        LevelManager.Instance.SaveCurrentLevelResults(
            CurrentScore, 
            TotalTime,
            GemFound,
            JamFound,
            GoldFound);
        if (LevelCompleted)
        {
            LevelManager.Instance.UnlockNextLevel();
        }
    }

    public void NextLevel()
    {
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f;
        
        LevelManager.Instance.GotoLevel(LevelManager.Instance.CurrentLevelData.Number + 1);
    }
    
    public void Restart()
    {
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f;
        
        LevelManager.Instance.GotoLevel(LevelManager.Instance.CurrentLevelData.Number);
    }
    
    public void BackToManuMenu()
    {
        m_inputReader.DisablePlayerInput();
        m_inputReader.EnableUIInput();
        Cursor.lockState = CursorLockMode.None;
        
        Time.timeScale = 1f;
        
        LevelManager.Instance.GotoMainMenu();
    }
}
