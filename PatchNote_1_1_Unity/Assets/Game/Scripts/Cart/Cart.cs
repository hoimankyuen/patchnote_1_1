using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using MoonlightTools.AudioSystem;
using MoonlightTools.GeneralTools;
using MoonlightTools.MathTools;
using QuickerEffects;
using Unity.Collections;
using UnityEngine;

public class Cart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader m_inputReader;
    [SerializeField] private Transform m_cameraTransform;
    [SerializeField] private CameraManager m_cameraManager;
    
    [Header("Components")]
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_cameraFollowTargetTransform;
    [SerializeField] private List<Collider> m_wheelColliders;
    [SerializeField] private List<Transform> m_wheelModelTransforms;
    [SerializeField] private ParticleSystem m_speedEffect;
    [SerializeField] private List<ParticleSystem> m_wheelEffects;
    [SerializeField] private Overlay m_overlay;

    [Header("Settings (Acceleration)")] 
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration;
    [SerializeField] private float m_deceleration;

    [Header("Settings (Turning)")]
    [SerializeField] private float m_maxTurningSpeed;
    [SerializeField] private float m_turningForce;

    [Header("Settings (Appearance)")]
    [SerializeField] private float m_effectMinSpeed;
    [SerializeField] private float m_effectMaxSpeed;
    [SerializeField] private Color m_speedEffectColor;
    [SerializeField] private Color m_wheelEffectColor;
    [SerializeField] private float m_collisionEffectSpeed;
    [SerializeField] private Color m_speedBlinkColor;
    [SerializeField] private float m_speedBlinkDuration;


    private Vector2 m_MoveInput;
    private float m_RotateInput;
    
    private readonly List<int> m_colliderInstanceIds = new List<int>();
    
    private Vector3 m_originalPosition;
    private Quaternion m_originalRotation;
    private Vector3 m_lastPosition;

    private bool m_speedBlinkEffectPlayed;
    private Coroutine m_speedBlinkCoroutine;
    
    public float MaxSpeed => m_maxSpeed;
    
    public float CurrentSpeed { get; private set; }
    public event Action CurrentSpeedChanged;
    
    // ======== Unity Events ========
    
    private void Awake()
    {
        RecordOriginalTransform();
        SetupColliders();
        Physics.ContactModifyEvent += PreventGhostCollisions;
    }

    private void Start()
    {
        m_inputReader.Move += OnMoveTrolley;
        m_inputReader.Rotate += OnRotateTrolley;
        m_inputReader.Reset += OnResetTrolley;
    }
    
    private void OnDestroy()
    {
        if (m_inputReader != null)
        {
            m_inputReader.Move -= OnMoveTrolley;
            m_inputReader.Rotate -= OnRotateTrolley;
            m_inputReader.Reset -= OnResetTrolley;
        }
        Physics.ContactModifyEvent -= PreventGhostCollisions;
    }

    private void Update()
    {
        CalculateSpeed();
        ControlSpeedEffect();
        ControlWheelEffects();
        ControlSpeedBlinkEffect();
    }
    
    private void FixedUpdate()
    {
        ApplyMovement();
        ReOrientateCameraFollowTarget();

        RotateWheels();
    }
    
    // ======== Physics System Handling ========
    
    private void SetupColliders()
    {
        foreach (Collider c in m_wheelColliders)
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
    
    // ======== Reset ========

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
        m_lastPosition = transform.position;
        
        CurrentSpeed = 0;
        CurrentSpeedChanged?.Invoke();
    }

    // ======== Controls ========

    private void OnMoveTrolley(Vector2 inputValue)
    {
        m_MoveInput = inputValue;
    }

    private void OnRotateTrolley(float inputValue)
    {
        m_RotateInput = inputValue;
    }
    
    private void ApplyMovement()
    {
        // Calculate movement direction relative to camera
        Vector3 cameraForward = Vector3.ProjectOnPlane( m_cameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(m_cameraTransform.right, Vector3.up).normalized;
        Vector3 moveDir = (cameraForward * m_MoveInput.y + cameraRight * m_MoveInput.x).normalized;
            
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
            m_rigidbody.AddRelativeTorque(Vector3.up * (m_RotateInput * m_turningForce), ForceMode.Acceleration);
        }
    }
    
    private void ReOrientateCameraFollowTarget()
    {
        m_cameraFollowTargetTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane( transform.forward, Vector3.up).normalized, Vector3.up);
    }

    private void CalculateSpeed()
    {
        float newCurrentSpeed = m_rigidbody.linearVelocity.magnitude;
        if (!Mathf.Approximately(newCurrentSpeed, CurrentSpeed))
        {
            CurrentSpeed = m_rigidbody.linearVelocity.magnitude;
            CurrentSpeedChanged?.Invoke();
        }
    }
    
    // ======== Appearance ========

    private void RotateWheels()
    {
        if (Vector3.Distance(transform.position, m_lastPosition) > 0.01f)
        {
            Vector3 direction = Vector3.ProjectOnPlane(transform.position - m_lastPosition, transform.up).normalized;
            foreach (Transform wheel in m_wheelModelTransforms)
            {
                wheel.rotation = Quaternion.LookRotation(direction, transform.up);
            }
            m_lastPosition = transform.position;
        }
    }

    private void ControlSpeedEffect()
    {
        if (CurrentSpeed > m_effectMinSpeed)
        {
            m_speedEffect.Play();
            
            ParticleSystem.MainModule main = m_speedEffect.main;
            main.startSpeed = new ParticleSystem.MinMaxCurve(CurrentSpeed * -1);
            Color effectColor = m_speedEffectColor;
            effectColor.a *= Mathf.InverseLerp(m_effectMinSpeed, m_effectMaxSpeed, CurrentSpeed);
            main.startColor = new ParticleSystem.MinMaxGradient(effectColor);
            
            ParticleSystem.ShapeModule shape = m_speedEffect.shape;
            shape.rotation = Quaternion.LookRotation(transform.InverseTransformDirection(m_rigidbody.linearVelocity), transform.up).eulerAngles;
        }
        else
        {
            m_speedEffect.Stop();
        }
    }

    private void ControlWheelEffects()
    {
        foreach (ParticleSystem wheelEffect in m_wheelEffects)
        {
            ParticleSystem.MainModule main = wheelEffect.main;
            Color effectColor = m_wheelEffectColor;
            effectColor.a *= Mathf.InverseLerp(m_effectMinSpeed, m_effectMaxSpeed, CurrentSpeed);
            main.startColor = new ParticleSystem.MinMaxGradient(effectColor);
        }
    }

    private void ControlSpeedBlinkEffect()
    {
        if (CurrentSpeed > m_collisionEffectSpeed)
        {
            if (!m_speedBlinkEffectPlayed)
            {
                if (m_speedBlinkCoroutine != null)
                {
                    StopCoroutine(m_speedBlinkCoroutine);
                    m_speedBlinkCoroutine = null;
                }
                m_speedBlinkCoroutine = StartCoroutine(SpeedBlinkEffectSequence());
                AudioManager.Instance.PlaySoundEffect("Dash01");
                m_speedBlinkEffectPlayed = true;
            }
        }
        else
        {
            m_speedBlinkEffectPlayed = false;
        }
    }

    private IEnumerator SpeedBlinkEffectSequence()
    {
        Color color = m_speedBlinkColor;
        m_overlay.Color = color;
        yield return CoroutineUtils.LerpWithTime(m_speedBlinkDuration, t =>
        {
            color.a = Mathfx.Sinerp(1f, 0f, t);
            m_overlay.Color = color;
        });
        m_speedBlinkCoroutine = null;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Item")) 
            return;
        
        AudioManager.Instance.PlaySoundEffect("Trolley_impact01");
        if (other.relativeVelocity.magnitude > m_collisionEffectSpeed)
        {
            m_cameraManager.ShakeTrolleyCamera();
        }
    }
}