using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        // here for controlling other system that uses input actions independently
        [SerializeField] private InputActionAsset m_inputActionAsset; 
        
        private InputSystem_Actions m_inputActions;
        
        public UnityAction<Vector2> Move = delegate {  };
        public UnityAction<float> Rotate = delegate { };
        public UnityAction Look = delegate {  };
        public UnityAction Reset = delegate { };
        public UnityAction Pause = delegate { };
        
        public void EnablePlayerInput()
        {
            if (m_inputActions == null)
            {
                m_inputActions = new InputSystem_Actions();
                m_inputActions.Player.SetCallbacks(this);
                
            }
            m_inputActions.Player.Enable();
            m_inputActionAsset.actionMaps[0].Enable();
        }

        public void EnableUIInput()
        {
            m_inputActionAsset.actionMaps[1].Enable();
        }
        
        public void DisablePlayerInput()
        {
            m_inputActions.Player.Disable();
            m_inputActionAsset.actionMaps[0].Disable();
        }

        public void DisableUIInput()
        {
            m_inputActionAsset.actionMaps[1].Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnRotate(InputAction.CallbackContext context)
        {
            Rotate?.Invoke(context.ReadValue<float>());
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke();
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            Reset?.Invoke();
        }
        
        public void OnPause(InputAction.CallbackContext context)
        {
            Pause?.Invoke();
        }
    }
}