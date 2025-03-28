﻿using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Editor.Node;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VariableBoard = RobertHoudin.Framework.Core.Primitives.DataContainers.VariableBoard;

namespace RobertHoudin.Framework.Editor.Misc
{

#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class InspectorView : VisualElement
#else
    public class InspectorView : VisualElement
#endif
    {
        private UnityEditor.Editor editor;

        public void UpdateSelection(RhNodeView nodeView)
        {
            Clear();
            Object.DestroyImmediate(editor);
            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            var container = new IMGUIContainer(() => {
                if (editor.target)
                    editor.OnInspectorGUI();
            });
            Add(container);
        }

        /*
        public void UpdateBlackBoard(RhTree tree)
        {
            Clear();
            Object.DestroyImmediate(editor);
            if (tree is null || tree.blackboard is null) return;
            editor = UnityEditor.Editor.CreateEditor(tree.blackboard);
            var container = new IMGUIContainer(() => {
                if (editor.target)
                    editor.OnInspectorGUI();
            });
            Add(container);
        }*/
        public void UpdateVariableBoard(VariableBoard variableBoard)
        {
            Clear();
            Object.DestroyImmediate(editor);
            if (variableBoard is null) return;
            editor = UnityEditor.Editor.CreateEditor(variableBoard);
            var container = new IMGUIContainer(() => {
                if (editor.target)
                    editor.OnInspectorGUI();
            });
            Add(container);
        }

        public void UpdateAgent(IRhPropertyBlock agent)
        {
            /*
            Clear();
            Object.DestroyImmediate(editor);
            if (agent is null) return;
            editor = UnityEditor.Editor.CreateEditor(agent);
            var container = new IMGUIContainer(() => {
                if (editor.target)
                    editor.OnInspectorGUI();
            });
            Add(container);
            */
        }
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }
#endif
    }
}