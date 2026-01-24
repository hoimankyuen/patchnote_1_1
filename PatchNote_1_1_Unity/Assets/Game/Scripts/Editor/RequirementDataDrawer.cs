using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequirementData))]
public class RequirementDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty itemType = property.FindPropertyRelative("itemType");
        SerializedProperty quantity = property.FindPropertyRelative("quantity");

        float itemTypeHeight = EditorGUI.GetPropertyHeight(itemType);
        float quantityHeight = EditorGUI.GetPropertyHeight(quantity);
            
        Rect itemTypeRect = new Rect(position.x, position.y ,position.width / 2f, position.height);
        Rect quantityRect = new Rect(position.x + position.width / 2f, position.y, position.width / 2f,  position.height);
            
        EditorGUI.PropertyField(itemTypeRect, itemType, new GUIContent());
        EditorGUI.PropertyField(quantityRect, quantity, new GUIContent());
            
        EditorGUI.EndProperty();
    }
}
