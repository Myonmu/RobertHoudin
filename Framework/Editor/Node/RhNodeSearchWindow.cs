using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Editor.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RobertHoudin.Framework.Editor.Node
{
    public class RhNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public RhTreeEditor.RhTreeEditor targetWindow;

        public RhTreeView targetView;
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry> {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")), 
                new SearchTreeGroupEntry(new GUIContent("RH Nodes"), 1)
            };
            var types = TypeCache.GetTypesDerivedFrom<RhNode>();
            AddEntry(tree, types, 2);
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var worldMousePosition = targetWindow.rootVisualElement.ChangeCoordinatesTo(
                targetWindow.rootVisualElement.parent, context.screenMousePosition - targetWindow.position.position);
            var localMousePosition = targetView.contentViewContainer.WorldToLocal(worldMousePosition);
            targetView.CreateNodeAtPosition((Type)searchTreeEntry.userData, localMousePosition);
            return true;
        }

        private static void AddEntry(List<SearchTreeEntry> treeEntries, TypeCache.TypeCollection typeCollection, int level)
        {
            Dictionary<string, List<Type>> menuStructure = new();
            foreach (var type in typeCollection) {
                if(type.IsAbstract||type.IsInterface||type.IsGenericType||type.IsGenericTypeDefinition) continue;
                var fullName = type.FullName?.Split(".");
                if (fullName == null) continue;
                var str = fullName[^2];
                if (!menuStructure.ContainsKey(str))
                    menuStructure.Add(str, new List<Type>());
                if (type.IsAbstract) continue;
                menuStructure[str].Add(type);
            }
            foreach (var (key, value) in menuStructure.Where(entry => entry.Value.Count != 0)) {
                treeEntries.Add(new SearchTreeGroupEntry(new GUIContent(key), level));
                treeEntries.AddRange(value.Select(node =>
                    new SearchTreeEntry(new GUIContent(SpacesFromCamel(node.Name.Replace("Node", "")))) {
                        userData = node, level = level + 1
                    }));
            }

        }

        private static string SpacesFromCamel(string value)
        {
            if (value.Length <= 0)
                return value;
            var result = new List<char>();
            var array = value.ToCharArray();
            foreach (var item in array) {
                if (char.IsUpper(item) && result.Count > 0)
                    result.Add(' ');
                result.Add(item);
            }

            return new string(result.ToArray());
        }
    }
}