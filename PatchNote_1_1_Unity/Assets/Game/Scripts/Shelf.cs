using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using MoonlightTools.MathTools;
using UnityEngine;
using UnityEngine.Events;

public class Shelf : MonoBehaviour
{
    private static readonly int Collide = Animator.StringToHash("Collide");
    
    [Header("References")]
    [SerializeField] private ItemLibrary m_itemLibrary;
    [SerializeField] private ItemList m_itemList;
    
    [Header("Components")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<Transform> m_itemSpawnPositions;
    [SerializeField] private Transform m_launchDirection;
    
    [Header("Settings")]
    [SerializeField] private float m_forceRequiredForItemsFall = 1f;
    [SerializeField] private RangedFloat m_forceOffShelfRandomRange;
    [SerializeField] private RangedFloat m_launchDirectionRandomRange;
    
    private List<Rigidbody> m_items = new List<Rigidbody>();
    
    private bool m_launched = false;
    
    // ==== Unity Events ====

    public UnityEvent OnShelfHit;

    private void Start()
    {
        SetupItems();
    }

    private void OnDrawGizmos()
    {
        if (m_launchDirection != null)
        {
            Gizmos.color = Color.green;
            GizmoUtils.DrawArrow(
                m_launchDirection.position,
                m_launchDirection.position + m_launchDirection.forward * 1f,
                0.5f,
                m_launchDirection.up);
        }

        Gizmos.color = Color.yellow;
        Vector3 positionAverage = Vector3.zero;
        int positionCount = 0;
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            if (itemSpawnPosition != null)
            {
                Gizmos.DrawWireSphere(itemSpawnPosition.position, 0.1f);
                positionAverage += itemSpawnPosition.position;
                positionCount++;
            }
        }

        if (m_itemList != null)
        {
            Gizmos.color = Color.red;
            GizmoUtils.DrawText(positionAverage / positionCount, m_itemList.name, FontStyle.Bold);
        }
    }

    private void SetupItems()
    {
        ConstructedWeightedList();
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            ItemType itemType = m_itemList.WeightedItemTypes[Random.Range(0, m_itemList.WeightedItemTypes.Count)];
            ItemData itemData = m_itemLibrary.GetItemData(itemType);

            GameObject newObject = Instantiate(itemData.Prefab,
                itemSpawnPosition.position,
                itemSpawnPosition.rotation,
                transform);
            m_items.Add(newObject.GetComponent<Rigidbody>());
            newObject.GetComponent<Item>().SetData(itemData);
        }
    }

    private void ConstructedWeightedList()
    {
        if (m_itemList.WeightedItemTypes != null)
            return;
        
        m_itemList.WeightedItemTypes = new List<ItemType>();
        foreach (ItemType itemType in m_itemList.ItemTypes)
        {
            ItemData itemData = m_itemLibrary.GetItemData(itemType);
            int weight = itemData.IsBonus ? 1 : itemData.IsRare ? 8 : 16;
            for (int i = 0; i < weight; i++)
            {
                m_itemList.WeightedItemTypes.Add(itemType);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_launched)
            return;
        
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        
        if (collision.relativeVelocity.magnitude < m_forceRequiredForItemsFall) return;
        
        OnShelfHit?.Invoke();

        m_animator.SetTrigger(Collide);
        
        ActivatePhysicsForItems();

        m_launched = true;
    }

    private void ActivatePhysicsForItems()
    {
        foreach (Rigidbody item in m_items)
        {
            item.isKinematic = false;
            Vector3 direction = Quaternion.AngleAxis(m_launchDirectionRandomRange.Random(), m_launchDirection.up) * m_launchDirection.forward;
            float force = m_forceOffShelfRandomRange.Random();
            item.linearVelocity = direction * force;
        }
    }
}