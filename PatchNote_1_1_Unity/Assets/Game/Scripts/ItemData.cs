using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Data", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType type;
    public float price;
    public float mass;
    public Sprite icon;
    public GameObject prefab;
}
