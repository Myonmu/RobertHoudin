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
    public class SoBindingSourceEntry : BindingSourceEntryBase
    {
        public void Reflect(SerializedProperty prop)
        {
            properties.Clear();
            subProperties.Clear();

            prop.FindPropertyRelative("selectedProperty").stringValue = null;
            prop.serializedObject.ApplyModifiedProperties();
            prop.serializedObject.Update();

            resetDelegates?.Invoke();


            var obj = prop.FindPropertyRelative("obj").boxedValue;
            if (obj is null) return;

            PopulateFirstProp(obj);

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
            var obj = prop.FindPropertyRelative("obj").boxedValue;
            PopulateSecondProp(obj, selectedProp);

            ClampSubProperty(prop);
        }


        public bool ReEvaluate( /*SerializedProperty prop*/)
        {
            ScriptableObject selectedComp;
            var target = propertyObject as BindingSource;
            try
            {
                //try to check if object is null or missing ref
                selectedComp = target.UnityObj as ScriptableObject;
                //((ScriptableObject)prop.FindPropertyRelative("obj").boxedValue);
            }
            catch
            {
                WipeAll( /*prop*/);
                return false;
            }

            //Now check property modification
            if (selectedComp is null) return false;
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
        
    }
}