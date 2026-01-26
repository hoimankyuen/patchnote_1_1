using System;
using System.Collections.Generic;
using MoonlightTools.AudioSystem;
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

    public static Action OnCashedItems;
    
    private int m_goalLayer;
    
    private void Awake()
    {
        PreprocessLayers();
    }

    private void PreprocessLayers()
    {
        m_goalLayer = LayerMask.NameToLayer("Goal");
    }
    
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
        if (other.gameObject.layer != m_goalLayer) 
            return;

        if (Items.Count <= 0)
            return;
        
        m_gameManager.SolidifyProgress(Items, TotalScore);
        
        AudioManager.Instance.PlaySoundEffect("Ka_ching01");
        OnCashedItems?.Invoke();
        
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