using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankPathingSystem;

[CustomPropertyDrawer(typeof(NeighborConnection))]
public class ConnectionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = property.FindPropertyRelative("neighborName").stringValue;
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("neighborID"), GUIContent.none);
        EditorGUI.indentLevel = 0;
        EditorGUI.PropertyField(new Rect(contentPosition.x+50, contentPosition.y, contentPosition.width, contentPosition.height), property.FindPropertyRelative("disttoneighbor"));
    }
}
