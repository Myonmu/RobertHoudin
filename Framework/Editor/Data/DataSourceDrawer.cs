using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEditor;
using UnityEngine;

namespace RobertHoudin.Framework.Editor.Data
{
    [CustomPropertyDrawer(typeof(DataSource<>))]
    public class DataSourceDrawer : PropertyDrawer
    {
        /* Doesn't work correctly
        private static VisualTreeAsset _treeAsset;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _treeAsset ??= AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                (AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:VisualTreeAsset DataSourceView")[0]));
            var root = _treeAsset.CloneTree();
            root.Q<Label>("FieldName").text = property.name;
            return root;
        }
        */


        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            GUI.Box(position, GUIContent.none);
            var enumVal = property.FindPropertyRelative("sourceType");

            var maxWidth = position.width;
            var xmin = position.xMin;
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(position, property.displayName);
            position.y += position.height * 1.1f;
            position.width *= 0.5f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("sourceName"), GUIContent.none);
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("sourceType"), GUIContent.none);

            if (enumVal.enumValueIndex < 0) enumVal.enumValueIndex = 0;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type != "None")
            {
                return;
            }

            position.width = maxWidth;
            position.y += EditorGUIUtility.singleLineHeight;
            position.x = xmin;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("value"), new GUIContent("Value"));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumVal = property.FindPropertyRelative("sourceType");
            if (enumVal.enumValueIndex < 0) return EditorGUIUtility.singleLineHeight;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type == "None")
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value")) +
                       EditorGUIUtility.singleLineHeight * 2.5f;
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}