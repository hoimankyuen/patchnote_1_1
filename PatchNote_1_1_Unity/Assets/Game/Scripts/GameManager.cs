using Input;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Playing,
        Paused,
    }

    [Header("References")]
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private CinemachineCamera m_camera;
    [SerializeField] private Cart m_cart;
 
    public State CurrentState { get; private set; }
    
    private void Start()
    {
        m_inputReader.Pause += Pause;
        m_inputReader.Resume += Resume;
        
        //m_inputReader.DisablePlayerInput();
        //m_inputReader.DisableUIInput();
        
        m_inputReader.EnablePlayerInput();
        
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        if (m_inputReader != null)
        {
            m_inputReader.Pause -= Pause;
            m_inputReader.Resume -= Resume;
            
            m_inputReader.DisablePlayerInput();
        }

        //Cursor.lockState = CursorLockMode.None;
    }

    
    // ======== Pausing ======== 
    
    private void Pause()
    {
        if (CurrentState != State.Playing)
            return;
        
        CurrentState = State.Paused;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (CurrentState != State.Paused)
            return;
        
        CurrentState = State.Playing;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    
}
