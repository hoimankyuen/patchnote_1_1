using System;
using System.Collections.Generic;
using UnityEngine;

public class CartItems : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager m_gameManager;
    
    [Header("Components")]
    [SerializeField] private ParticleSystem m_cashEffect;
    
    public List<Item> Items { get; private set; } = new List<Item>();
    public event Action ItemsChanged;
    
    public float TotalScore { get; private set; }
    public event Action TotalScoreChanged;
    
    public void AddItem(Item item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            UpdateCurrentItemsTotalValue();
            ItemsChanged?.Invoke();
        }
    }

    public void RemoveItem(Item item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            UpdateCurrentItemsTotalValue();
            ItemsChanged?.Invoke();
        }
    }

    private void UpdateCurrentItemsTotalValue()
    {
        float total = 0f;
        
        foreach (Item item in Items)
        {
            total += item.Price;
        }

        TotalScore = total;
        TotalScoreChanged?.Invoke();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Goal")) 
            return;

        if (Items.Count <= 0)
            return;
        
        m_gameManager.SolidifyProgress(Items, TotalScore);
        
        //TODO: We may want to have particle effects for each item here, should definitely have a cash register/"cha-ching!" sound effect   
        foreach (Item item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();
        ItemsChanged?.Invoke();
        
        TotalScore = 0f;
        TotalScoreChanged?.Invoke();
        
        m_cashEffect.Play();
    }
}