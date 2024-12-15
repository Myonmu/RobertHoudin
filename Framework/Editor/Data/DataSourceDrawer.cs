using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEditor;
using UnityEngine;
using GUIContent = UnityEngine.GUIContent;

namespace RobertHoudin.Framework.Editor.Data
{
    [CustomPropertyDrawer(typeof(DataSource<>))]
    public class DataSourceDrawer : PropertyDrawer
    {
        /// <summary>
        /// Dummy to make nameof work, doesn't do anything else;
        /// </summary>
        private struct _
        {
        }
        /* Doesn't work correctly, paints eye-draining UI.
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
            var enumVal = property.FindPropertyRelative(nameof(DataSource<_>.sourceType));
            if (enumVal.enumValueIndex < 0) enumVal.enumValueIndex = 0;
            var type = enumVal.enumValueFlag;

            var maxWidth = position.width;
            var xmin = position.xMin;
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(position, property.displayName);
            position.y += position.height * 1.1f;
            position.width *= 0.5f;
            if (type != (int)SourceType.PropertyBlock)
            {
                EditorGUI.LabelField(position, "(binding ignored)");
            }
            else
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(DataSource<_>.sourceName)), GUIContent.none);
            }
            position.x += position.width;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(DataSource<_>.sourceType)), GUIContent.none);

            
            if (type != (int)SourceType.None)
            {
                return;
            }

            position.width = maxWidth;
            position.y += EditorGUIUtility.singleLineHeight;
            position.x = xmin;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(DataSource<_>.value)), new GUIContent("Value"));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumVal = property.FindPropertyRelative(nameof(DataSource<_>.sourceType));
            if (enumVal.enumValueIndex < 0) return EditorGUIUtility.singleLineHeight;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type == "None")
                return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(DataSource<_>.value))) +
                       EditorGUIUtility.singleLineHeight * 2.5f;
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}