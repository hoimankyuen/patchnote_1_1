using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions, IUIActions
    {
        // here for controlling other system that uses input actions independently
        [SerializeField] private InputActionAsset m_inputActionAsset; 
        
        private InputSystem_Actions m_inputActions;
        
        public UnityAction<Vector2> Move = delegate {  };
        public UnityAction<float> Rotate = delegate { };
        public UnityAction Look = delegate {  };
        public UnityAction Reset = delegate { };
        public UnityAction Pause = delegate { };
        public UnityAction Continue = delegate { };
        public UnityAction Resume = delegate { };

        private void InitialiseInputActions()
        {
            if (m_inputActions != null) 
                return;
            
            m_inputActions = new InputSystem_Actions();
            m_inputActions.Player.SetCallbacks(this);
            m_inputActions.UI.SetCallbacks(this);
        }
        
        public void EnablePlayerInput()
        {
            InitialiseInputActions();
            m_inputActions.Player.Enable();
            m_inputActionAsset.actionMaps[0].Enable();
        }

        public void EnableUIInput()
        {
            InitialiseInputActions();
            m_inputActions.UI.Enable();
            m_inputActionAsset.actionMaps[1].Enable();
        }
        
        public void DisablePlayerInput()
        {
            InitialiseInputActions();
            m_inputActions.Player.Disable();
            m_inputActionAsset.actionMaps[0].Disable();
        }

        public void DisableUIInput()
        {
            InitialiseInputActions();
            m_inputActions.UI.Disable();
            m_inputActionAsset.actionMaps[1].Disable();
        }
        
        // ======== Player Input Callbacks ========

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
            if (context.phase != InputActionPhase.Performed)
                return;

            Reset?.Invoke();
        }
        
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            
            Pause?.Invoke();
        }

        // ======== UI Input Callbacks ========
        
        public void OnNavigate(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            // Not in used here
        }

        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            
            Resume?.Invoke();
        }
        
        public void OnContinue(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Performed)
                return;
            
            if (context.phase != InputActionPhase.Performed)
                return;
            
            Continue?.Invoke();
        }
    }
}