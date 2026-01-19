using Input;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_cameraTransform;

    [Header("Settings")] 
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_deceleration;
    [SerializeField] private float m_turnSpeed = 100f;

    private float m_currentSpeed;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        m_inputReader.EnablePlayerActions();
    }

    private void OnDisable()
    {
        m_inputReader.DisablePlayerActions();
    }

    private void FixedUpdate()
    {
        if (m_inputReader.IsMoveInputPressed)
        {
            m_rigidbody.linearVelocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
        
            // Get camera directions flattened on the Y axis
            Vector3 cameraForward = m_cameraTransform.forward;
            Vector3 cameraRight = m_cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement direction relative to camera
            Vector3 moveDir = (cameraForward * m_inputReader.MoveDirection.y) + (cameraRight * m_inputReader.MoveDirection.x);
                
            Vector3 newPosition = m_rigidbody.position + moveDir * (m_acceleration * Time.fixedDeltaTime);
            m_rigidbody.MovePosition(newPosition);

            float turnAngle = m_inputReader.MoveDirection.x * m_turnSpeed * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, turnAngle, 0f);
            m_rigidbody.MoveRotation(m_rigidbody.rotation * deltaRotation);
        }
    }
}