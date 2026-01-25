using System;
using System.Collections.Generic;
using System.Linq;

// ======== Individual Entry ========

[System.Serializable]
public struct ItemQuantity
{
    public ItemType Type;
    public int Quantity;

    public ItemQuantity(ItemType type, int quantity)
    {
        Type = type;
        Quantity = quantity;
    }

    public ItemQuantity(ItemQuantity other)
    {
        Type = other.Type;
        Quantity = other.Quantity;
    }

    public static ItemQuantity Invalid => new ItemQuantity(ItemType.None, 0);
    
    public void SetQuantity(int value)
    {
        Quantity = value;
    }

    public bool IsValid()
    {
        return Type != ItemType.None;
    }
}

// ======== Collection ========

public class ItemQuantities
{
    private readonly Dictionary<ItemType, int> m_collection = new();
    
    public int Count => m_collection.Count;
    
    // ======== Entries Management ========
    
    public void AddRange(List<ItemQuantity> pairs)
    {
        foreach (ItemQuantity pair in pairs)
        {
            m_collection.Add(pair.Type, pair.Quantity);
        }
    }
    
    public void Clear()
    {
        m_collection.Clear();
    }

    // ======== Values Managements ========
    
    public int GetQuantity(ItemType itemType)
    {
        return m_collection.GetValueOrDefault(itemType, 0);
    }
    
    public void SetQuantityIfExists(ItemType itemType, int quantity)
    {
        if (m_collection.ContainsKey(itemType))
        {
            m_collection[itemType] = quantity;
        }
    }
    
    public void ChangeQuantityIfExists(ItemType itemType, int quantity)
    {
        if (m_collection.ContainsKey(itemType))
        {
            m_collection[itemType] += quantity;
        }
    }
    
    // ======== Inquiry ========
    
    public List<ItemQuantity> ToList()
    {
        return m_collection.Select(pair => new ItemQuantity(pair.Key, pair.Value)).ToList();
    }
    
    public bool TrueForAll(Func<ItemQuantity, bool> predicate)
    {
        return m_collection.All(kvp => predicate(new ItemQuantity(kvp.Key, kvp.Value)));
    }
}