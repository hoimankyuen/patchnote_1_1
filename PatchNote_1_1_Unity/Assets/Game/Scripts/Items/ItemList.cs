using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Data/Item List", order = 1)]
public class ItemList : ScriptableObject
{
    public List<ItemType> ItemTypes;
}