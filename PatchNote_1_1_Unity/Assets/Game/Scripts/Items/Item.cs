using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    private ItemData m_data;

    public void SetData(ItemData itemData)
    {
        m_data = itemData;
    }

    public float GetItemPrice => m_data.Price;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        other.TryGetComponent(out CartItems cartItems);
        cartItems.AddItem(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        
        other.TryGetComponent(out CartItems cartItems);
        cartItems.RemoveItem(this);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

        Destroy(gameObject);
    }
}