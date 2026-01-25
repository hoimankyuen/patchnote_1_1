using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "ScriptableObjects/ItemList", order = 1)]
public class ItemList : ScriptableObject
{
    public List<ItemData> Items;
}
