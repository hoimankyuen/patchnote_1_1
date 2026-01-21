using System.Collections.Generic;
using Input;
using Unity.Collections;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_cameraTransform;
    [SerializeField] private List<Collider> m_wheeLColliders;

    [Header("Settings (Acceleration)")] 
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_deceleration;

    [Header("Settings Turning")]
    [SerializeField] private float m_maxTurningSpeed;
    [SerializeField] private float m_turningForce;

    private float m_currentSpeed;

    private readonly List<int> _colliderInstanceIds = new List<int>();

    private void Awake()
    {
        SetupColliders();
        Physics.ContactModifyEvent += PreventGhostCollisions;
    }
    
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

    private void OnDestroy()
    {
        Physics.ContactModifyEvent -= PreventGhostCollisions;
    }

    private void FixedUpdate()
    {
        if (m_inputReader.IsMoveInputPressed)
        {
            // Get camera directions flattened on the Y axis
            Vector3 cameraForward = Vector3.ProjectOnPlane( m_cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(m_cameraTransform.right, Vector3.up).normalized;
            //Vector3 cameraForward = m_cameraTransform.forward;
            //Vector3 cameraRight = m_cameraTransform.right;
            //cameraForward.y = 0;
            //cameraRight.y = 0;
            //cameraForward.Normalize();
            //cameraRight.Normalize();

            // Calculate movement direction relative to camera
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
    
    private void SetupColliders()
    {
        foreach (Collider c in m_wheeLColliders)
        {
            _colliderInstanceIds.Add(c.GetInstanceID());
            c.hasModifiableContacts = true;
            c.providesContacts = true;
        }
    }
    
    private void PreventGhostCollisions(PhysicsScene scene, NativeArray<ModifiableContactPair> contactPairs)
    {
        foreach (ModifiableContactPair contactPair in contactPairs)
        {
            if (!_colliderInstanceIds.Contains(contactPair.colliderInstanceID))
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
}