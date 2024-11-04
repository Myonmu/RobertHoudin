using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using UnityEditor;
using UnityEngine;

namespace MochiBTS.Editor.VariableDrawers
{
    [CustomPropertyDrawer(typeof(SoBindingSource<>))]
    public class SoBindingSourceDrawer : BindingSourceDrawerBase
    {
        private static readonly Regex MatchArrayElement = new(@"data\[(\d+)\]$");

        //private static readonly Dictionary<string, BindingSourceEntry> Entries = new();
        private static readonly Dictionary<int, Dictionary<string, SoBindingSourceEntry>> Entries = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entry = Initialize(property);
            var maxWidth = position.width;
            EditorGUI.BeginProperty(position, label, property);
            position.width = 0.4f * maxWidth;
            EditorGUI.BeginChangeCheck();
            property.serializedObject.Update();
            EditorGUI.PropertyField(position, property.FindPropertyRelative("obj"), GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                //Debug.Log($"{property.propertyPath},idx{index},init{initialized}");
                property.serializedObject.Update();
                entry.Reflect(property);
                entry.UpdateSelected(property);
                entry.SubReflect(property);
                entry.UpdateSelected(property);
                entry.bind?.Invoke();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }

            position.width = 0.3f * maxWidth;
            position.x += 0.4f * maxWidth;
            if (entry.properties is not null)
            {
                if (entry.properties.Count < 1)
                {
                    EditorGUI.HelpBox(position, "Unavailable", MessageType.Warning);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    property.serializedObject.Update();
                    entry.selectedPropertyIndex = EditorGUI.Popup(position, entry.selectedPropertyIndex,
                        entry.properties.ToArray());
                    property.serializedObject.ApplyModifiedProperties();
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.Update();
                        entry.UpdateSelected(property);
                        entry.SubReflect(property);
                        entry.UpdateSelected(property);
                        entry.bind?.Invoke();
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    }
                }
            }

            position.x += 0.3f * maxWidth;
            if (entry.subProperties is not null)
            {
                if (entry.subProperties.Count < 1)
                {
                    EditorGUI.HelpBox(position, "Unavailable", MessageType.Warning);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    property.serializedObject.Update();
                    entry.selectedSubIndex =
                        EditorGUI.Popup(position, entry.selectedSubIndex, entry.subProperties.ToArray());
                    property.serializedObject.ApplyModifiedProperties();
                    if (EditorGUI.EndChangeCheck())
                    {
                        property.serializedObject.Update();
                        entry.UpdateSelected(property);
                        entry.bind?.Invoke();
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                    }
                }
            }

            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static void ReEvaluateBinding(SerializedProperty prop)
        {
            //if (prop is null) return;
            var path = prop.propertyPath + ".soBindingSource";
            var id = prop.serializedObject.targetObject.GetInstanceID();
            if (Entries.ContainsKey(id) && Entries[id].ContainsKey(path))
            {
                var target = Entries[id][path];
                target.ReEvaluate( /*prop*/);
            }
        }


        protected static SoBindingSourceEntry Initialize(SerializedProperty prop)
        {
            if (prop is null) return null;
            var serializedObject = prop.serializedObject;
            var path = prop.propertyPath;
            var id = prop.serializedObject.targetObject.GetInstanceID();

            if (Entries.ContainsKey(id) && Entries[id].ContainsKey(path))
            {
                var target = Entries[id][path];
                return target;
            }

            SoBindingSourceEntry entry = new();
            //if (initialized) return;
            //Debug.Log($"Initialize {id+path}");
            entry.propertyObject = serializedObject == null || serializedObject.targetObject == null
                ? null
                : serializedObject.targetObject;
            entry.objectType = entry.propertyObject?.GetType();
            if (!string.IsNullOrEmpty(path) && entry.propertyObject != null)
            {
                var splitPath = path.Split('.');
                Type fieldType = null;

                //work through the given property path, node by node
                for (var i = 0; i < splitPath.Length; i++)
                {
                    var pathNode = splitPath[i];

                    //both arrays and lists implement the IList interface
                    if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType))
                    {
                        //IList items are serialized like this: `Array.data[0]`
                        Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal),
                            serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

                        //just skip the `Array` part of the path
                        pathNode = splitPath[++i];

                        //match the `data[0]` part of the path and extract the IList item index
                        var elementMatch = MatchArrayElement.Match(pathNode);
                        if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out entry.id))
                        {
                            var objectArray = (IList)entry.propertyObject;
                            var validArrayEntry = objectArray != null && entry.id < objectArray.Count;
                            entry.propertyObject = validArrayEntry ? objectArray[entry.id] : null;
                            entry.objectType = fieldType.IsArray
                                ? fieldType.GetElementType() //only set for arrays
                                : objectArray?[entry.id].GetType();
                        }
                        else
                        {
                            Debug.LogErrorFormat(serializedObject.targetObject,
                                "Unexpected path format for array item: '{0}'", pathNode);
                        }

                        //reset fieldType, so we don't end up in the IList branch again next iteration
                        fieldType = null;
                    }
                    else
                    {
                        FieldInfo field;
                        var instanceType = entry.objectType;
                        var fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.FlattenHierarchy;
                        do
                        {
                            field = instanceType.GetField(pathNode, fieldBindingFlags);

                            //b/c a private, serialized field of a subclass isn't directly retrievable,
                            fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                            //if necessary, work up the inheritance chain until we find it.
                            instanceType = instanceType.BaseType;
                        } while (field == null && instanceType != typeof(object));

                        //store object info for next iteration or to return
                        entry.propertyObject = field == null || entry.propertyObject == null
                            ? null
                            : field.GetValue(entry.propertyObject);
                        fieldType = field == null ? null : field.FieldType;
                        entry.objectType = fieldType;
                    }
                }
            }

            if (entry.propertyObject != null)
            {
                entry.bind = (Action)entry.propertyObject.GetType().GetMethod("Bind")
                    ?.CreateDelegate(typeof(Action), entry.propertyObject);
                entry.resetDelegates = (Action)entry.propertyObject.GetType().GetMethod("ResetDelegate")
                    ?.CreateDelegate(typeof(Action), entry.propertyObject);
                entry.getValue = (Func<object>)entry.propertyObject.GetType().GetMethod("BoxedValue")
                    ?.CreateDelegate(typeof(Func<object>), entry.propertyObject);
                entry.getValueType = (Func<Type>)entry.propertyObject.GetType().GetMethod("GetValueType")
                    ?.CreateDelegate(typeof(Func<Type>), entry.propertyObject);
            }

            //initialized = true;
            if (!Entries.ContainsKey(id))
            {
                Entries.Add(id, new Dictionary<string, SoBindingSourceEntry>());
            }

            Entries[id].Add(path, entry);
            entry.resetDelegates?.Invoke();
            if (entry.ReEvaluate( /*prop*/)) return entry;
            entry.Reflect(prop);
            entry.bind?.Invoke();
            return entry;
        }

        public static SoBindingSourceEntry GetEntry(SerializedProperty prop)
        {
            var path = prop.propertyPath;
            var id = prop.serializedObject.targetObject.GetInstanceID();
            if (!Entries.ContainsKey(id) || !Entries[id].ContainsKey(path)) return null;
            return Entries[id][path];
        }

        public static void InvalidateCache()
        {
            Entries.Clear();
        }

        public static void InitWhenHidden(SerializedProperty prop)
        {
            Initialize(prop);
        }


        public static void InvalidateCache(SerializedProperty prop)
        {
            var id = prop.serializedObject.targetObject.GetInstanceID();
            if (!Entries.ContainsKey(id)) return;
            Entries[id].Clear();
        }

        public static string FetchValue(SerializedProperty prop)
        {
            //return "";
            var path = prop.propertyPath;
            var id = prop.serializedObject.targetObject.GetInstanceID();
            if (!Entries.ContainsKey(id) || !Entries[id].ContainsKey(path)) return "NULL";
            var target = Entries[id][path];
            if (target.getValue is null) return "Not Bound";
            var result = target.getValue.Invoke();
            return result is null ? "NULL" : result.ToString();
        }
    }
}