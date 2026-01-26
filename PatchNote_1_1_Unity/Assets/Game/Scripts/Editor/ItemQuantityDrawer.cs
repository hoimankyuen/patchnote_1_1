using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemQuantity))]
public class ItemQuantityDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty itemType = property.FindPropertyRelative("Type");
        SerializedProperty quantity = property.FindPropertyRelative("Quantity");
        
        Rect itemTypeRect = new Rect(position.x, position.y ,position.width / 2f, position.height);
        Rect quantityRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f,  position.height);
            
        EditorGUI.PropertyField(itemTypeRect, itemType, new GUIContent());
        EditorGUI.PropertyField(quantityRect, quantity, new GUIContent());
            
        EditorGUI.EndProperty();
    }
}
