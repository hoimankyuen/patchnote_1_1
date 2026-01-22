using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private static readonly int Fall = Animator.StringToHash("Fall");
    
    [Header("References")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<Transform> m_itemSpawnPositions;
    [SerializeField] private ItemList m_itemList;
    
    [Header("Settings")]
    [SerializeField] private float m_forceRequiredForItemsFall = 1f;
    [SerializeField] private float m_forceOffShelf = 5f;

    private List<Rigidbody> m_items = new List<Rigidbody>();
    
    private void Start()
    {
        SetupItems();
    }

    private void SetupItems()
    {
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            m_items.Add(Instantiate(m_itemList.Items[Random.Range(0, m_itemList.Items.Count)],
                itemSpawnPosition.position,
                itemSpawnPosition.rotation,
                transform).GetComponent<Rigidbody>());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            
        other.gameObject.TryGetComponent(out Rigidbody rb);
        
        if (rb.linearVelocity.magnitude < m_forceRequiredForItemsFall) return;

        m_animator.SetTrigger(Fall);
        
        ActivatePhysicsForItems();
    }

    private void ActivatePhysicsForItems()
    {
        foreach (Rigidbody item in m_items)
        {
            item.isKinematic = false;
            item.AddForce(item.transform.forward * m_forceOffShelf);
        }
    }
}