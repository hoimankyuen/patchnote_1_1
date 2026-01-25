using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Data", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType Type;
    public float Price;
    public float Mass;
    public Sprite Icon;
    public GameObject Prefab;
}
