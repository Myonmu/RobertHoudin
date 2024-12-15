using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEditor;
using UnityEngine;
namespace RobertHoudin.Framework.Editor.VariableDrawers
{
    [CustomPropertyDrawer(typeof(Number))]
    public class NumberDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Number.value)));
        }
    }
}