using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Library", fileName = "ItemLibrary")]
public class ItemLibrary : ScriptableObject
{
    [SerializeField] private List<ItemData> items;

    private Dictionary<ItemType, ItemData> m_runtimeItemDataLookup;
    
    public ItemData GetItemData(ItemType type)
    {
        if (m_runtimeItemDataLookup == null)
        {
            m_runtimeItemDataLookup = new Dictionary<ItemType, ItemData>();
            foreach (ItemData itemData in items)
            {
                m_runtimeItemDataLookup[itemData.type] = itemData;
            }
        }
        
        return m_runtimeItemDataLookup[type];
    }
}
