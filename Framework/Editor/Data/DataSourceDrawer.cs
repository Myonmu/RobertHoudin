using System;
using System.Linq;
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
        public static event Action OnDataSourceTypeChanged;
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

        private void DrawBindingSelector(ref Rect position, SerializedProperty property)
        {
            var window = EditorWindow.focusedWindow as RhTreeEditor.RhTreeEditor;
             var sourceName = property.FindPropertyRelative(nameof(DataSource<_>.sourceName));
            if (window?.Tree is null)
            {
                EditorGUI.LabelField(position, sourceName.stringValue);
            }
            else if (window?.Tree?.propertyBlockType is null)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(DataSource<_>.sourceName)), GUIContent.none);
            }
            else
            {
                var blockType = window.Tree.propertyBlockType.GetType();
                // extract the expected type
                var dsType = property.boxedValue.GetType();
                while (!dsType.IsGenericType)
                {
                    dsType = dsType.BaseType ?? throw new Exception($"Failed to get reflection data from data source type: " +
                                                                    $"no generic type in {property.boxedValue.GetType()}'s hierarchy.");
                }
                var typeArg = dsType.GetGenericArguments()[0];
                var fields = ReflectionUtils.GetFieldsAssignableTo(typeArg, blockType).Select(x=>x.Name).ToArray();
                var i = 0;
                // determine current index
                for (i = 0; i < fields.Length; i++)
                {
                    if (fields[i] == sourceName.stringValue)
                    {
                        break;
                    }
                }
                var newIndex = EditorGUI.Popup(position, i, fields);
                if (newIndex != i)
                {
                    sourceName.stringValue = fields[newIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            GUI.Box(position, GUIContent.none);
            var enumVal = property.FindPropertyRelative(nameof(DataSource<_>.sourceType));
            if (enumVal.enumValueIndex < 0) enumVal.enumValueIndex = 0;
            var type = (SourceType)enumVal.enumValueFlag;

            var maxWidth = position.width;
            var xmin = position.xMin;
            position.height = EditorGUIUtility.singleLineHeight;
            GUI.Label(position, property.displayName);
            position.y += position.height * 1.1f;
            position.width *= 0.5f;
            if (type != SourceType.PropertyBlock)
            {
                EditorGUI.LabelField(position, "(binding ignored)");
            }
            else
            {
                DrawBindingSelector(ref position, property);
            }
            position.x += position.width;
            var newEnumVal = (SourceType)EditorGUI.EnumPopup(position, type);
            if (newEnumVal != type)
            {
                type = newEnumVal;
                property.FindPropertyRelative(nameof(DataSource<_>.sourceType)).enumValueFlag = (int)newEnumVal;
                property.serializedObject.ApplyModifiedProperties();
                OnDataSourceTypeChanged?.Invoke();
            }

            if (type != SourceType.None)
            {
                return;
            }

            position.width = maxWidth;
            position.y += EditorGUIUtility.singleLineHeight;
            position.x = xmin;
            var val = property.FindPropertyRelative(nameof(DataSource<_>.value));
            if (val != null)
                EditorGUI.PropertyField(position, val, new GUIContent("Value"));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumVal = property.FindPropertyRelative(nameof(DataSource<_>.sourceType));
            if (enumVal.enumValueIndex < 0) return EditorGUIUtility.singleLineHeight;
            var type = enumVal.enumNames[enumVal.enumValueIndex];
            if (type == "None")
            {
                var val = property.FindPropertyRelative(nameof(DataSource<_>.value));
                if(val != null)  
                    return EditorGUI.GetPropertyHeight(val) + EditorGUIUtility.singleLineHeight * 2.5f;
            }
                
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}