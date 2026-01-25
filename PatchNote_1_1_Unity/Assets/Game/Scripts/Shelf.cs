using System.Collections.Generic;
using MoonlightTools.GizmoTools;
using MoonlightTools.MathTools;
using UnityEngine;

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
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            if (itemSpawnPosition != null)
            {
                Gizmos.DrawWireSphere(itemSpawnPosition.position, 0.1f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (m_itemSpawnPositions.Count <= 0 || m_itemList == null || m_itemList.ItemTypes.Count <= 0)
            return;
        
        Gizmos.color = Color.yellow;
        Vector3 positionAverage = Vector3.zero;
        int positionCount = 0;
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            if (itemSpawnPosition != null)
            {
                positionAverage += itemSpawnPosition.position;
                positionCount++;
            }
        }
        if (positionCount > 0)
        {
            string text = "";
            for (int i = 0; i < m_itemList.ItemTypes.Count; i++)
            {
                text += $"{m_itemList.ItemTypes[i].ToString()}{(i < m_itemList.ItemTypes.Count - 1 ? "," : "")}";
            }
            GizmoUtils.DrawText(positionAverage / positionCount, text);
        }
    }

    private void SetupItems()
    {
        foreach (Transform itemSpawnPosition in m_itemSpawnPositions)
        {
            ItemType itemType = m_itemList.ItemTypes[Random.Range(0, m_itemList.ItemTypes.Count)];
            ItemData itemData = m_itemLibrary.GetItemData(itemType);
            GameObject newObject = Instantiate(itemData.Prefab,
                itemSpawnPosition.position,
                itemSpawnPosition.rotation,
                transform);
            m_items.Add(newObject.GetComponent<Rigidbody>());
            newObject.GetComponent<Item>().SetData(itemData);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_launched)
            return;
        
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            
        //collision.gameObject.TryGetComponent(out Rigidbody rb);
        
        if (collision.relativeVelocity.magnitude < m_forceRequiredForItemsFall) return;

        m_animator.SetTrigger(Collide);
        
        ActivatePhysicsForItems();

        m_launched = true;
    }

    private void ActivatePhysicsForItems()
    {
        foreach (Rigidbody item in m_items)
        {
            item.isKinematic = false;
            //item.AddForce(item.transform.forward * m_forceOffShelf);
            Vector3 direction = Quaternion.AngleAxis(m_launchDirectionRandomRange.Random(), m_launchDirection.up) * m_launchDirection.forward;
            float force = m_forceOffShelfRandomRange.Random();
            item.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}