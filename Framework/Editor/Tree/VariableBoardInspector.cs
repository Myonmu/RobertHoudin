using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using RobertHoudin.Framework.Editor.VariableDrawers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RobertHoudin.Framework.Editor.Tree
{
    [CustomEditor(typeof(VariableBoard))]
    public class VariableBoardInspector : UnityEditor.Editor
    {
        private VariableBoard drawer;
        private readonly List<string> varNames = new();
        private bool needRevalidateNames = true;
        private readonly Dictionary<int, bool> namingConflictStats = new();
        private ReorderableList list;
        private int selectedTypeIndex = 0;
        private static readonly Dictionary<string, Type> TypeCache = new();
        private static string[] typeNameCache;
        private bool forceFolding = false;

        private void OnEnable()
        {
            drawer = target as VariableBoard;
            FetchAllVariableTypes();
            typeNameCache = TypeCache.Keys.ToArray();
            needRevalidateNames = true;
            SerializedProperty prop = null;
            try
            {
                prop = serializedObject.FindProperty("composites");
            }
            catch
            {
                return;
            }

            list = new ReorderableList(serializedObject, prop,
                true, true, true, true)
            {
                //list.elementHeight = EditorGUIUtility.singleLineHeight * 2.5f;
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Variables")
            };

            list.drawElementCallback =
                (rect, index, isActive, isFocused) =>
                {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    var cachedStat = namingConflictStats.TryGetValue(index, out var n);
                    var namingConflict = !needRevalidateNames && (!cachedStat || n);
                    rect.x += 10f;
                    rect.width -= 10f;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    var key = element.FindPropertyRelative("key").stringValue;
                    if (needRevalidateNames)
                    {
                        if (varNames.Any(n => n == key))
                        {
                            namingConflict = true;
                        }

                        if (!namingConflict) varNames.Add(key);
                        namingConflictStats.Add(index, namingConflict);
                    }

                    var val = element.FindPropertyRelative("val");
                    var useBinding = element.FindPropertyRelative("bindVariable");
                    //var useBinding = element.managedReferenceValue

                    string mode = null;
                    mode = useBinding.enumNames[useBinding.enumValueIndex];
                    var valString =val.isArray?"<Array>": $"{val.boxedValue}";
                    var boundValueIsNull = false;
                    if (mode != "Value")
                    {
                        valString = mode switch
                        {
                            "GO" => ValStringFromGo(element, out boundValueIsNull),
                            "SO" => ValStringFromSo(element, out boundValueIsNull),
                            _ => valString
                        };
                    }

                    var voidName = string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key);
                    if (namingConflict || voidName)
                    {
                        GUI.contentColor = Color.red;
                    }
                    else if (mode != "Value") GUI.contentColor = boundValueIsNull ? Color.red : Color.cyan;
                    else GUI.contentColor = Color.yellow;

                    element.isExpanded = EditorGUI.Foldout(rect, element.isExpanded,
                        voidName
                            ? $" !!! EMPTY VARIABLE NAME !!!"
                            : namingConflict
                                ? $" !!! NAMING CONFLICT !!!"
                                : $" {element.FindPropertyRelative("key").stringValue}     = <{val.type}> {valString}",
                        true)&&!forceFolding;

                    GUI.contentColor = Color.white;
                    if (element.isExpanded)
                    {
                        rect.x += 10f;
                        rect.y += EditorGUIUtility.singleLineHeight;
                        rect.width -= 10f;
                        rect.height -= EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(rect, element, true);
                    }

                    if ((list.count - 1) == index)
                    {
                        if (needRevalidateNames) needRevalidateNames = false;
                        if (forceFolding) forceFolding = false;
                    }
                };


            list.elementHeightCallback = (index) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                var h = EditorGUIUtility.singleLineHeight;
                if (element.isExpanded)
                    h += EditorGUI.GetPropertyHeight(element);
                return h;
            };


            list.onChangedCallback += (l) =>
            {
                //Debug.Log("ChangeDetected");
                SoBindingSourceDrawer.InvalidateCache(list.serializedProperty);
                GoBindingSourceDrawer.InvalidateCache(list.serializedProperty);
                ReEvaluateNaming();
                //Repaint();
            };

            list.onSelectCallback += l =>
            {
                foreach (var index in l.selectedIndices)
                {
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    var bind = element.FindPropertyRelative("bindVariable");
                    var enumVal = bind.enumNames[bind.enumValueIndex];
                    if (enumVal == "Value") continue;
                    var bindingSource = element.FindPropertyRelative("bindingSource");

                    SoBindingSourceDrawer.ReEvaluateBinding(element);
                    GoBindingSourceDrawer.ReEvaluateBinding(element);
                    ReEvaluateNaming();
                }
            };

            list.onAddCallback = reorderableList =>
            {
                Debug.Log(">>>>> Use the button at the top of the list.");
            };
        }

        private void ReEvaluateNaming()
        {
            varNames.Clear();
            namingConflictStats.Clear();
            needRevalidateNames = true;
        }

        private static string ValStringFromGo(SerializedProperty element, out bool boundValueIsNull)
        {
            var bindingSource = element.FindPropertyRelative("goBindingSource");
            GoBindingSourceDrawer.InitWhenHidden(bindingSource);
            var objOrig = (GameObject)bindingSource.FindPropertyRelative("obj").boxedValue;
            var obj = objOrig is null ? "null" : objOrig.name;
            var compOrig = ((Component)bindingSource.FindPropertyRelative("selectedComponent").boxedValue);
            var comp = compOrig is null ? "null" : compOrig.GetType().Name;
            var prop = bindingSource.FindPropertyRelative("selectedProperty").stringValue;
            var sub = bindingSource.FindPropertyRelative("selectedSub").stringValue;
            var valString =
                $"{obj}.{comp}.{prop + (string.IsNullOrEmpty(sub) ? "" : $".{sub}")} => {GoBindingSourceDrawer.FetchValue(bindingSource)}";
            boundValueIsNull = string.IsNullOrEmpty(prop) ||
                               (string.IsNullOrEmpty(prop) && string.IsNullOrEmpty(sub));
            return valString;
        }

        private static string ValStringFromSo(SerializedProperty element, out bool boundValueIsNull)
        {
            var bindingSource = element.FindPropertyRelative("soBindingSource");
            SoBindingSourceDrawer.InitWhenHidden(bindingSource);
            var objOrig = (ScriptableObject)bindingSource.FindPropertyRelative("obj").boxedValue;
            var obj = objOrig is null ? "null" : objOrig.name;
            var prop = bindingSource.FindPropertyRelative("selectedProperty").stringValue;
            var sub = bindingSource.FindPropertyRelative("selectedSub").stringValue;
            var valString =
                $"{obj}.{prop + (string.IsNullOrEmpty(sub) ? "" : $".{sub}")} => {SoBindingSourceDrawer.FetchValue(bindingSource)}";
            boundValueIsNull = string.IsNullOrEmpty(prop) ||
                               (string.IsNullOrEmpty(prop) && string.IsNullOrEmpty(sub));
            return valString;
        }

        public override void OnInspectorGUI()
        {
            selectedTypeIndex = EditorGUILayout.Popup("Type", selectedTypeIndex, TypeCache.Keys.ToArray());
            if (GUILayout.Button("Create Variable"))
            {
                ReEvaluateNaming();
                var v = Instantiate(TypeCache[typeNameCache[selectedTypeIndex]]);
                drawer.composites.Add(v);
                //OnEnable();
            }

            if (GUILayout.Button("Fold Everything"))
            {
                forceFolding = true;
            }

            serializedObject.Update();
            if (list is null) OnEnable();
            else
                list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private static void FetchAllVariableTypes()
        {
            TypeCache.Clear();
            var cache = UnityEditor.TypeCache.GetTypesDerivedFrom<IMochiVariableBase>();
            foreach (var t in cache)
            {
                if (t.IsAbstract || t.IsGenericTypeDefinition || t.IsGenericType) continue;
                TypeCache.Add(t.Name.Replace("Mochi",""), t);
            }
        }

        private static IMochiVariableBase Instantiate(Type varType)
        {
            return (IMochiVariableBase)Activator.CreateInstance(varType);
        }
    }
}