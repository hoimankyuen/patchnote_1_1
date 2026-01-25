using System.Collections;
using UnityEngine;

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

    private bool m_grounded;

    public void SetData(ItemData itemData)
    {
        m_data = itemData;
        m_rigidbody.mass = itemData.Mass;
    }

    public ItemType ItemType => m_data.Type;
    public float GetItemPrice => m_data.Price;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("CartItem")) return;

        if (other.TryGetComponent(out CartItems cartItems))
        {
            cartItems.AddItem(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("CartItem")) return;

        if (other.TryGetComponent(out CartItems cartItems))
        {
            cartItems.RemoveItem(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) 
            return;

        if (m_grounded)
            return;

        m_grounded = true;
        gameObject.layer = LayerMask.NameToLayer("FallenItem");
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