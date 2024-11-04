using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using UnityEditor;
using UnityEngine;
namespace MochiBTS.Editor.VariableDrawers
{
    [CustomPropertyDrawer(typeof(MochiVariable<>), true)]
    public class MochiVariableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = true;
            position.x += 7f;
            var maxWidth = position.width;
            var initX = position.x;

            //Key
            position.height = EditorGUIUtility.singleLineHeight;
            position.width = maxWidth * 0.3f;
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();
            EditorGUI.PropertyField(position, property.FindPropertyRelative("key"), GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();
            //Mode
            position.x += maxWidth * 0.3f + 7f;
            position.width = maxWidth * 0.1f;
            var mode = property.FindPropertyRelative("bindVariable");
            property.serializedObject.Update();
            EditorGUI.PropertyField(position, mode, GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();
            //Value field
            position.x += maxWidth * 0.1f + 7f;
            position.width = maxWidth * 0.6f - 21f;
            var enumMode = mode.enumNames[mode.enumValueIndex];
            GUI.enabled = enumMode == "Value";
            var val = property.FindPropertyRelative("val");
            property.serializedObject.Update();
            if (val.isArray || EditorGUI.GetPropertyHeight(val) > EditorGUIUtility.singleLineHeight) {
                position.x = initX;
                position.y += EditorGUIUtility.singleLineHeight;
                position.width = maxWidth;
                position.height = EditorGUI.GetPropertyHeight(val);
            }
            EditorGUI.PropertyField(position, val, GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();
            //Binding Source
            if (enumMode == "Value") {
                return;
            }

            if (!property.isExpanded) {
                return;
            }
            GUI.enabled = mode.boolValue;
            position.y += position.height + 7f;
            position.height = EditorGUIUtility.singleLineHeight;
            position.x = initX;
            position.width = maxWidth;
            switch (enumMode) {
                case "GO":
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("goBindingSource"), GUIContent.none);
                    break;
                case "SO":
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("soBindingSource"), GUIContent.none);
                    break;
            }

            GUI.enabled = true;
            EditorGUI.EndProperty();


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var val = property.FindPropertyRelative("val"); 
            var h = EditorGUIUtility.singleLineHeight * (property.isExpanded ? 2.5f : 1);
            if (property.FindPropertyRelative("bindVariable").enumValueIndex == 0) {
                if(EditorGUI.GetPropertyHeight(val) <= EditorGUIUtility.singleLineHeight*1.1 && !val.isArray)
                    return EditorGUIUtility.singleLineHeight;
                return property.isExpanded ? EditorGUIUtility.singleLineHeight * 1.2f + EditorGUI.GetPropertyHeight(val) :
                    EditorGUIUtility.singleLineHeight * 1.2f;
            }
            h += EditorGUI.GetPropertyHeight(val);
            return h;
        }
    }
}