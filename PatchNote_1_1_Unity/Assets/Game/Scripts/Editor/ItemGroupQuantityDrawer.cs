using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemGroupQuantity))]
public class ItemGroupQuantityDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty itemGroup = property.FindPropertyRelative("Group");
        SerializedProperty quantity = property.FindPropertyRelative("Quantity");
        
        Rect itemGroupRect = new Rect(position.x, position.y ,position.width / 2f, position.height);
        Rect quantityRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f,  position.height);
            
        EditorGUI.PropertyField(itemGroupRect, itemGroup, new GUIContent());
        EditorGUI.PropertyField(quantityRect, quantity, new GUIContent());
            
        EditorGUI.EndProperty();
    }
}