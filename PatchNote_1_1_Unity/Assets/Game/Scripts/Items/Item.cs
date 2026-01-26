using System.Collections;
using MoonlightTools.AudioSystem;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private MeshRenderer m_renderer;
    
    [Header("Settings")]
    [SerializeField] private float m_fallenTimeout;
    [SerializeField] private float m_fallenBlinkDuration;
    [SerializeField] private float m_fallenBlinkTimes;
    
    private ItemData m_data;
    
    private int m_groundLayer;
    private int m_cartItemLayer;
    private int m_fallenItemLayer;
    
    private bool m_grounded;
    
    public ItemType Type => m_data.Type;
    public float Price => m_data.Price;
    public Sprite Icon => m_data.Icon;
    
    public UnityEvent OnHitGround;

    private void Awake()
    {
        PreprocessLayers();
    }
    
    public void SetData(ItemData itemData)
    {
        m_data = itemData;
        m_rigidbody.mass = itemData.Mass;
    }
    
    private void PreprocessLayers()
    {
        m_groundLayer = LayerMask.NameToLayer("Ground");
        m_cartItemLayer = LayerMask.NameToLayer("CartItem");
        m_fallenItemLayer = LayerMask.NameToLayer("FallenItem");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != m_cartItemLayer) 
            return;

        if (other.TryGetComponent(out CartItems cartItems))
        {
            cartItems.AddItem(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != m_cartItemLayer)
            return;

        if (other.TryGetComponent(out CartItems cartItems))
        {
            cartItems.RemoveItem(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != m_groundLayer) 
            return;
        
        OnHitGround?.Invoke();

        if (m_grounded)
            return;

        m_grounded = true;
        gameObject.layer = m_fallenItemLayer;
        StartCoroutine(DisappearSequence());
    }

    private IEnumerator DisappearSequence()
    {
        yield return new WaitForSeconds(m_fallenTimeout);
        for (int i = 0; i < m_fallenBlinkTimes; i++)
        {
            m_renderer.enabled = false;
            yield return new WaitForSeconds(m_fallenBlinkDuration / 2f);
            m_renderer.enabled = true;
            yield return new WaitForSeconds(m_fallenBlinkDuration / 2f);
        }
        Destroy(gameObject);
    }
}