using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEditor;
using UnityEngine;

namespace RobertHoudin.Framework.Editor.Data
{
    [CustomPropertyDrawer(typeof(DataSource<>))]
    public class DataSourceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            GUI.Box(position, GUIContent.none);
            var enumVal = property.FindPropertyRelative("sourceType");
            
            var maxWidth = position.width;
            var xmin = position.xMin;
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.contentColor = Color.yellow;
            GUI.Label(position, "Data Source");
            position.y += position.height*1.1f;
            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("sourceName"), GUIContent.none);
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("sourceType"), GUIContent.none);
            
            if(enumVal.enumValueIndex < 0) enumVal.enumValueIndex = 0;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type != "None") {
                GUI.contentColor = Color.white;
                return;
            }
            position.width = maxWidth;
            position.y += EditorGUIUtility.singleLineHeight;
            position.x = xmin;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent("Value"));
            GUI.contentColor = Color.white;

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumVal = property.FindPropertyRelative("sourceType");
            if (enumVal.enumValueIndex < 0) return EditorGUIUtility.singleLineHeight;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type == "None") return EditorGUIUtility.singleLineHeight * 4.4f;
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}