using TATools.Misc;
using TATools.RobertHoudin.Misc;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TATools.RobertHoudin.Editor
{
    [CustomPropertyDrawer(typeof(SciFloat))]
    public class SciFloatDrawer : PropertyDrawer
    {
        private static VisualTreeAsset _visualTreeAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            _visualTreeAsset ??= AssetDbShorthand.FindAndLoadFirst<VisualTreeAsset>("SciFloatView");
            _visualTreeAsset.CloneTree(root);
            var floatField = root.Q<FloatField>();
            floatField.label = property.displayName;
            floatField.Bind(property.FindPropertyRelative("value").serializedObject);
            var multiplier = root.Q<Label>("multiplier");
            var power = property.FindPropertyRelative("power");
            multiplier.text = $"x {Mathf.Pow(10, power.intValue)}";
            var increase = root.Q<Button>("increase");
            increase.clicked += () =>
            {
                power.intValue++;
                power.serializedObject.ApplyModifiedProperties();
                multiplier.text = $"x {Mathf.Pow(10, power.intValue)}";
            };
            var decrease = root.Q<Button>("decrease");
            decrease.clicked += () =>
            {
                power.intValue--;
                power.serializedObject.ApplyModifiedProperties();
                multiplier.text = $"x {Mathf.Pow(10, power.intValue)}";
            };
            var zero = root.Q<Button>("zero");
            zero.clicked += () =>
            {
                power.intValue = 0;
                power.serializedObject.ApplyModifiedProperties();
                multiplier.text = $"x {Mathf.Pow(10, power.intValue)}";
            };
            return root;
        }
    }
}