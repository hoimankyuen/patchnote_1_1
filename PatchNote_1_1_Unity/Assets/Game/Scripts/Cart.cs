using System.Collections.Generic;
using Input;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Transform m_cameraTransform;
    
    [Header("Components")]
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_cameraFollowTargetTransform;
    [SerializeField] private List<Collider> m_wheeLColliders;

    [Header("Settings (Acceleration)")] 
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_deceleration;

    [Header("Settings Turning")]
    [SerializeField] private float m_maxTurningSpeed;
    [SerializeField] private float m_turningForce;

    private Vector3 m_originalPosition;
    private Quaternion m_originalRotation;
    private float m_currentSpeed;

    private readonly List<int> m_colliderInstanceIds = new List<int>();

    private void Awake()
    {
        RecordOriginalTransform();
        SetupColliders();
        Physics.ContactModifyEvent += PreventGhostCollisions;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        m_inputReader.Reset += OnResetTrolley;
    }
    
    private void OnDestroy()
    {
        if (m_inputReader != null)
        {
            m_inputReader.Reset -= OnResetTrolley;
        }
        Physics.ContactModifyEvent -= PreventGhostCollisions;
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
        ApplyMovement();
        ReOrientateCameraFollowTarget();
    }
    
    private void SetupColliders()
    {
        foreach (Collider c in m_wheeLColliders)
        {
            m_colliderInstanceIds.Add(c.GetInstanceID());
            c.hasModifiableContacts = true;
            c.providesContacts = true;
        }
    }
    
    private void PreventGhostCollisions(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs)
    {
        foreach (ModifiableContactPair contactPair in contactPairs)
        {
            if (!m_colliderInstanceIds.Contains(contactPair.colliderInstanceID))
                continue;
            
            for (int i = 0; i < contactPair.contactCount; i++)
            {
                if (contactPair.GetSeparation(i) > 0f)
                {
                    contactPair.SetNormal(i, Vector3.up);
                }
            }
        }
    }

    private void RecordOriginalTransform()
    {
        m_originalPosition = transform.position;
        m_originalRotation = transform.rotation;
    }
    
    private void OnResetTrolley()
    {
        m_rigidbody.linearVelocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        transform.position = m_originalPosition;
        transform.rotation = m_originalRotation;
    }

    private void ApplyMovement()
    {
        // Calculate movement direction relative to camera
        Vector3 cameraForward = Vector3.ProjectOnPlane( m_cameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(m_cameraTransform.right, Vector3.up).normalized;
        Vector3 moveDir = (cameraForward * m_inputReader.MoveDirection.y + cameraRight * m_inputReader.MoveDirection.x).normalized;
            
        // Add force while limit the maximum speed
        if (m_rigidbody.linearVelocity.magnitude > m_maxSpeed)
        {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxSpeed;
        }
        else
        {
            m_rigidbody.AddForce(moveDir * m_acceleration, ForceMode.Acceleration);
        }
        
        // Add torque while limit the maximum rotation
        if (m_rigidbody.angularVelocity.magnitude > m_maxTurningSpeed)
        {
            m_rigidbody.angularVelocity = m_rigidbody.angularVelocity.normalized * m_maxTurningSpeed;
        }
        else
        {
            m_rigidbody.AddRelativeTorque(Vector3.up * (m_inputReader.RotateInput * m_turningForce), ForceMode.Acceleration);
        }
    }
    
    private void ReOrientateCameraFollowTarget()
    {
        m_cameraFollowTargetTransform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}