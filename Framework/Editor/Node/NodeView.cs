﻿using System;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Editor.Settings;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RobertHoudin.Framework.Editor.Node
{
    public sealed class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        internal static RhTreeSettings settings;
        public readonly RhNode node;
        public Action<NodeView> onNodeSelected;
        public Action<NodeView> onSetOutputFlag;

        public VisualElement loopStartContainer;
        public VisualElement loopResultContainer;

        public NodeView(RhNode nodeRef, bool isOutput) :
            base(AssetDatabase.GetAssetPath(RhTreeSettings.GetOrCreateSettings().nodeXml))
        {
            var settings = RhTreeSettings.GetOrCreateSettings();
            styleSheets.Add(settings.nodeStyle);
            // Construct the node view from fed Node instance
            node = nodeRef;
            title = nodeRef.name.Replace("Node", "");
            viewDataKey = node.GUID;
            style.left = node.position.x;
            style.top = node.position.y;
            tooltip = node.Tooltip;

            loopStartContainer = this.Q<VisualElement>("loopStart");
            loopResultContainer = this.Q<VisualElement>("loopResult");


            CreateInputPorts();
            CreateOutputPorts();
            CreateOtherPorts();

            //Add base uss class labels
            SetupClasses();

            var serializedObject = new SerializedObject(node);
            //Set up description label
            var descriptionLabel = this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(serializedObject);

            //Set up info label
            var infoLabel = this.Q<Label>("info");
            //infoLabel.RegisterCallback<>;
            infoLabel.bindingPath = "info";
            infoLabel.Bind(serializedObject);

            var subInfoLabel = this.Q<Label>("subInfo");
            subInfoLabel.bindingPath = "subInfo";
            subInfoLabel.Bind(serializedObject);

            var outputFlag = this.Q<Button>("OutputFlag");
            outputFlag.clicked += () => { onSetOutputFlag(this); };
            if (isOutput)
            {
                outputFlag.AddToClassList("output-flag");
            }

            var icon = this.Q<VisualElement>("nodeIcon");
            if (settings == null) settings = RhTreeSettings.GetOrCreateSettings();
            var iconImage = settings != null ? settings.GetIcon(nodeRef) : null;
            if (iconImage == null)
                return;
            icon.style.backgroundImage = new StyleBackground(iconImage);


            //Debug.Log(iconImage.name);
        }

        /// <summary>
        ///     Add the node's base class label. Allows to apply uss styling.
        /// </summary>
        private void SetupClasses()
        {
        }

        private void CreateOutputPorts()
        {
            foreach (var outputPort in node.OutputPorts)
            {
                var output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
                    outputPort.AcceptedType);
                output.portName = "";
                output.tooltip = $"{output.name} ({outputPort.GetType().Name})";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                output.viewDataKey = outputPort.GUID;
                outputContainer.Add(output);
            }
        }

        private void CreateInputPorts()
        {
            foreach (var inputPort in node.InputPorts)
            {
                Port input;
                switch (inputPort)
                {
                    case IRhMultiPort:
                        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi,
                            inputPort.AcceptedType);
                        break;
                    default:
                        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single,
                            inputPort.AcceptedType);
                        break;
                }

                input.portName = "";
                input.tooltip = $"{inputPort.name} ({inputPort.GetType().Name})";
                input.style.flexDirection = FlexDirection.Column;
                input.viewDataKey = inputPort.GUID;
                inputContainer.Add(input);
            }
        }

        public void CreateOtherPorts()
        {
            foreach (var p in node.GetPortsWithAttribute<RhLoopItemPortAttribute>())
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
                    p.AcceptedType);

                port.portName = "";
                port.tooltip = $"{p.name} ({p.GetType().Name})";
                port.style.flexDirection = FlexDirection.Column;
                port.viewDataKey = p.GUID;
                loopStartContainer.Add(port);
            }
            
            foreach (var p in node.GetPortsWithAttribute<RhLoopResultPortAttribute>())
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                    p.AcceptedType);

                port.portName = "";
                port.tooltip = $"{p.name} ({p.GetType().Name})";
                port.style.flexDirection = FlexDirection.Column;
                port.viewDataKey = p.GUID;
                loopResultContainer.Add(port);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position)");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            onNodeSelected?.Invoke(this);
            node.UpdateInfo();
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            node.UpdateInfo();
        }

        public void SortChildren()
        {
            //var composite = node as CompositeNode;
            //if (composite && composite.children is not null)
            //    composite.children.Sort(SortByHorizontalPosition);
        }

        private static int SortByHorizontalPosition(RhNode x, RhNode y)
        {
            return x.position.x < y.position.x ? -1 : 1;
        }

        [InitializeOnLoadMethod]
        private static void ClearCache()
        {
            settings = RhTreeSettings.GetOrCreateSettings();
        }

        public void OnBecomeOutputNode()
        {
            var outputFlag = this.Q<Button>("OutputFlag");
            outputFlag.AddToClassList("output-flag");
        }

        public void OnBecomeNonOutputNode()
        {
            var outputFlag = this.Q<Button>("OutputFlag");
            outputFlag.RemoveFromClassList("output-flag");
        }
    }
}