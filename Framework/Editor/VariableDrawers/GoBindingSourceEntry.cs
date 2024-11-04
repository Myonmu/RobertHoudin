using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using UnityEditor;
using UnityEngine;

namespace MochiBTS.Editor.VariableDrawers
{
    [Serializable]
    public class GoBindingSourceEntry: BindingSourceEntryBase
    {
        public List<Component> components = new();
        public List<string> componentNames = new();
        public int selectedComponentIndex;
        
        public void Refresh(SerializedProperty prop)
        {
            componentNames.Clear();
            components.Clear();
            selectedComponentIndex = 0;
            prop.FindPropertyRelative("selectedComponent").boxedValue = null;
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();
            var obj = (GameObject)prop.FindPropertyRelative("obj").boxedValue;
            if (obj is null) return;
            try
            {
                foreach (var c in obj.GetComponents(typeof(Component)))
                {
                    var key = c.GetType().Name;
                    var occurence = 0;
                    while (componentNames.Contains(key + " " + occurence))
                    {
                        occurence += 1;
                    }

                    componentNames.Add(key + " " + occurence);
                    components.Add(c);
                }
            }
            catch
            {
                Debug.Log("Object Not Found");
                // ignored
            }
        }

        public void Reflect(SerializedProperty prop)
        {
            properties.Clear();
            subProperties.Clear();

            prop.FindPropertyRelative("selectedProperty").stringValue = null;
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();

            resetDelegates?.Invoke();

            if (selectedComponentIndex >= 0 && selectedComponentIndex < components.Count)
                prop.FindPropertyRelative("selectedComponent").boxedValue = components[selectedComponentIndex];
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();

            var selectedComponent = prop.FindPropertyRelative("selectedComponent").boxedValue;
            if (selectedComponent is null) return;

            PopulateFirstProp(selectedComponent);

            ClampMainProperty(prop);
        }




        public void SubReflect(SerializedProperty prop)
        {
            subProperties.Clear();
            prop.FindPropertyRelative("selectedSub").stringValue = null;
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();

            resetDelegates.Invoke();

            var selectedProp = prop.FindPropertyRelative("selectedProperty").stringValue;
            if (selectedProp is null) return;
            var selectedComponent = prop.FindPropertyRelative("selectedComponent").boxedValue;
            PopulateSecondProp(selectedComponent, selectedProp);

            ClampSubProperty(prop);
        }


        public bool ReEvaluate( /*SerializedProperty prop*/)
        {
            Component[] currentComps;
            var target = propertyObject as BindingSource;
            try
            {
                //try to check if object is null or missing ref
                currentComps = (target.BaseObj as GameObject).GetComponents(typeof(Component));
                //((GameObject)prop.FindPropertyRelative("obj").boxedValue).GetComponents(typeof(Component));
            }
            catch
            {
                WipeAll( /*prop*/);
                return false;
            }
            //The object is present, check if there is a change in component structure;

            var changeDetected = currentComps.Length != components.Count ||
                                 currentComps
                                     .Where((c, index) => c.GetInstanceID() != components[index].GetInstanceID()).Any();
            if (changeDetected)
            {
                components.Clear();
                componentNames.Clear();
                foreach (var c in currentComps)
                {
                    var key = c.GetType().Name;
                    var occurence = 0;
                    while (componentNames.Contains(key + " " + occurence))
                    {
                        occurence += 1;
                    }

                    componentNames.Add(key + " " + occurence);
                    components.Add(c);
                }
            }

            var prevSelectedCompId = (target.UnityObj as Component).GetInstanceID();
            //((Component)prop.FindPropertyRelative("selectedComponent").boxedValue).GetInstanceID();
            var prevSelectedPersists = false;
            for (var index = 0; index < currentComps.Length; index++)
            {
                if (currentComps[index].GetInstanceID() != prevSelectedCompId) continue;
                selectedComponentIndex = index;
                prevSelectedPersists = true;
                break;
            }

            if (!prevSelectedPersists)
            {
                target.UnityObj = null;
                //prop.FindPropertyRelative("selectedComponent").boxedValue = null;
                selectedPropertyIndex = 0;
            }

            //Now check property modification
            if (target.UnityObj is not Component selectedComp) return false;
            PopulateFirstProp(selectedComp);

            var selectedProperty = target.selectedProperty;
            //prop.FindPropertyRelative("selectedProperty").stringValue;
            if (properties.Contains(selectedProperty)) selectedPropertyIndex = properties.IndexOf(selectedProperty);
            if (properties.Count > 0 && selectedPropertyIndex >= properties.Count)
            {
                selectedPropertyIndex = 0;
            }

            UpdateSelected();

            //Check sub
            if (selectedProperty is null) return false;
            PopulateSecondProp(selectedComp, selectedProperty);

            var selectedSub = target.selectedSub;
            //prop.FindPropertyRelative("selectedSub").stringValue;
            if (subProperties.Contains(selectedSub)) selectedSubIndex = subProperties.IndexOf(selectedSub);
            if (subProperties.Count > 0 && selectedSubIndex >= subProperties.Count)
            {
                selectedSubIndex = 0;
            }

            UpdateSelected();

            bind.Invoke();
            return true;
        }

        protected override void WipeAll(SerializedProperty prop)
        {
            prop.FindPropertyRelative("selectedComponent").boxedValue = null;
            selectedComponentIndex = 0;
            components.Clear();
            componentNames.Clear();
            base.WipeAll(prop);
        }

        protected override void WipeAll()
        {
            var target = propertyObject as BindingSource;

            components.Clear();
            componentNames.Clear();
            target.UnityObj = null;

            selectedComponentIndex = 0;
            properties.Clear();
            selectedComponentIndex = 0;
            target.selectedProperty = null;

            target.selectedSub = null;

            selectedSubIndex = 0;
            subProperties.Clear();
            resetDelegates.Invoke();
        }
    }
}