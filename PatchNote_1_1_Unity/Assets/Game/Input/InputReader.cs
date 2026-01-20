using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

namespace Input
{
    public interface IInputReader
    {
        Vector2 MoveDirection { get; }
        void EnablePlayerActions();
    }
    
    [CreateAssetMenu(fileName = "InputReader", menuName = "Input/InputReader")]
    public class InputReader : ScriptableObject, IInputReader, IPlayerActions
    {
        public UnityAction<Vector2> Move = delegate {  };
        public UnityAction Attack = delegate {  };
        public UnityAction Next = delegate {  };
        public UnityAction Previous = delegate {  };
        public UnityAction Look = delegate {  };
        public UnityAction Sprint = delegate {  };
        public UnityAction<float> Rotate = delegate { };
        
        private InputSystem_Actions m_inputActions;
        
        public Vector2 MoveDirection => m_inputActions.Player.Move.ReadValue<Vector2>();
        public Vector2 LookDirection => m_inputActions.Player.Look.ReadValue<Vector2>();
        
        public bool IsMoveInputPressed => m_inputActions.Player.Move.IsPressed();
        public float RotateInput => m_inputActions.Player.Rotate.ReadValue<float>();
        
        public void EnablePlayerActions()
        {
            if (m_inputActions == null)
            {
                m_inputActions = new InputSystem_Actions();
                m_inputActions.Player.SetCallbacks(this);
            }
            m_inputActions.Enable();
        }
        
        public void DisablePlayerActions()
        {
            m_inputActions.Disable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Attack?.Invoke();
            }
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
            Previous?.Invoke();
        }
        
        public void OnNext(InputAction.CallbackContext context)
        {
            Next?.Invoke();
        }
        
        public void OnSprint(InputAction.CallbackContext context)
        {
            Sprint?.Invoke();
        }

        public void OnRotate(InputAction.CallbackContext context)
        {
            Rotate?.Invoke(context.ReadValue<float>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke();
        }
    }
}