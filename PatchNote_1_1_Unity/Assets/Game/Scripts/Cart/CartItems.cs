using System;
using System.Collections.Generic;
using UnityEngine;

public class CartItems : MonoBehaviour
{
    //TODO: Unserialize these fields after testing
    
    [SerializeField] private List<Item> m_items = new List<Item>();

    [SerializeField] private float m_currentItemsTotalValue;
    
    public void AddItem(Item item)
    {
        if (!m_items.Contains(item))
        {
            m_items.Add(item);
            UpdateCurrentItemsTotalValue();
        }
    }

    public void RemoveItem(Item item)
    {
        if (m_items.Contains(item))
        {
            m_items.Remove(item);
            UpdateCurrentItemsTotalValue();
        }
    }

    private void UpdateCurrentItemsTotalValue()
    {
        float total = 0f;
        
        foreach (Item item in m_items)
        {
            total += item.GetItemPrice;
        }

        m_currentItemsTotalValue = total;
    }

    public float GetPriceOfAllItemsInCart => m_currentItemsTotalValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Goal")) return;
        
        //TODO: Increase score by m_currentItemsTotalValue
        
        //TODO: We may want to have particle effects for each item here, should definitely have a cash register/"cha-ching!" sound effect   
        foreach (Item item in m_items)
        {
            Destroy(item.gameObject);
        }
        m_items.Clear();
        m_currentItemsTotalValue = 0f;
    }
}