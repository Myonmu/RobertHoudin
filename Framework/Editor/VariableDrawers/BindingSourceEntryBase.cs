using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using UnityEditor;

namespace RobertHoudin.Framework.Editor.VariableDrawers
{
    public class BindingSourceEntryBase
    {
        public int id;
        public object propertyObject;
        public Type objectType;
        public Action bind;
        public Action resetDelegates;
        public Func<Type> getValueType;
        public Func<object> getValue;
        public List<string> properties = new();
        public int selectedPropertyIndex;
        public List<string> subProperties = new();
        public int selectedSubIndex;

        protected void PopulateFirstProp(object selectedComponent)
        {
            var propertyInfos = selectedComponent.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType == getValueType.Invoke() ||
                    propertyInfo.PropertyType.GetProperties().Any(info => info.PropertyType == getValueType.Invoke()))
                {
                    properties.Add(propertyInfo.Name);
                }
            }

            var fieldInfos = selectedComponent.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.FieldType == getValueType.Invoke() ||
                    fieldInfo.FieldType.GetProperties().Any(info => info.PropertyType == getValueType.Invoke()))
                {
                    properties.Add(fieldInfo.Name);
                }
            }
        }

        protected void PopulateSecondProp(object selectedComponent, string selectedProp)
        {
            var propertyInfo = selectedComponent.GetType().GetProperty(selectedProp);
            var subProps = new PropertyInfo[] { };
            var subfields = new FieldInfo[] { };
            if (propertyInfo != null)
            {
                if (propertyInfo.PropertyType == getValueType.Invoke()) subProperties.Add("<None>");
                subProps = propertyInfo.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                subfields = propertyInfo.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            var fieldInfo = selectedComponent.GetType().GetField(selectedProp);
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType == getValueType.Invoke()) subProperties.Add("<None>");
                subProps = fieldInfo.FieldType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                subfields = fieldInfo.FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            }

            foreach (var sub in subProps)
            {
                if (sub.PropertyType == getValueType.Invoke())
                {
                    subProperties.Add(sub.Name);
                }
            }

            foreach (var subfield in subfields)
            {
                if (subfield.FieldType == getValueType.Invoke())
                {
                    subProperties.Add(subfield.Name);
                }
            }
        }

        protected void ClampMainProperty(SerializedProperty prop)
        {
            if (properties.Count > 0 && selectedPropertyIndex >= properties.Count)
            {
                selectedPropertyIndex = 0;
                prop.FindPropertyRelative("selectedProperty").stringValue = properties[0];
                prop.serializedObject.ApplyModifiedProperties();
                prop.serializedObject.Update();
            }

            if (properties.Count > selectedPropertyIndex)
            {
                prop.FindPropertyRelative("selectedProperty").stringValue = properties[selectedPropertyIndex];
                prop.serializedObject.ApplyModifiedProperties();
                prop.serializedObject.Update();
            }
        }

        protected void ClampSubProperty(SerializedProperty prop)
        {
            if (subProperties.Count > 0 && selectedSubIndex >= subProperties.Count)
            {
                selectedPropertyIndex = 0;
                prop.FindPropertyRelative("selectedSub").stringValue = subProperties[0];
                prop.serializedObject.ApplyModifiedProperties();
                prop.serializedObject.Update();
            }

            if (subProperties.Count > selectedSubIndex)
            {
                prop.FindPropertyRelative("selectedSub").stringValue = subProperties[selectedSubIndex];
                prop.serializedObject.ApplyModifiedProperties();
                prop.serializedObject.Update();
            }
        }

        public void UpdateSelected()
        {
            var target = propertyObject as BindingSource;
            if (properties.Count > selectedPropertyIndex)
                target.selectedProperty = properties[selectedPropertyIndex];
            if (subProperties.Count > selectedSubIndex)
                target.selectedSub = subProperties[selectedSubIndex];
        }

        public void UpdateSelected(SerializedProperty prop)
        {
            if (properties.Count > selectedPropertyIndex)
                prop.FindPropertyRelative("selectedProperty").stringValue = properties[selectedPropertyIndex];
            if (subProperties.Count > selectedSubIndex)
                prop.FindPropertyRelative("selectedSub").stringValue = (subProperties[selectedSubIndex] == "<None>"
                    ? null
                    : subProperties[selectedSubIndex]);
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();
        }

        protected virtual void WipeAll(SerializedProperty prop)
        {
            prop.FindPropertyRelative("obj").boxedValue = null;

            properties.Clear();
            prop.FindPropertyRelative("selectedProperty").stringValue = null;

            prop.FindPropertyRelative("selectedSub").stringValue = null;

            selectedSubIndex = 0;
            subProperties.Clear();
            resetDelegates.Invoke();
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();
        }

        protected virtual void WipeAll()
        {
            var target = propertyObject as BindingSource;

            properties.Clear();
            target.selectedProperty = null;

            target.selectedSub = null;

            selectedSubIndex = 0;
            subProperties.Clear();
            resetDelegates.Invoke();
        }
    }
}