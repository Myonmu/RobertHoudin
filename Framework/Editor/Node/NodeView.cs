using System;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace MochiBTS.Editor
{
    public sealed class NodeView : Node
    {
        internal static RhTreeSettings settings;
        public readonly RhNode node;
        public Action<NodeView> onNodeSelected;

        public NodeView(RhNode nodeRef) :
            base(AssetDatabase.GetAssetPath(RhTreeSettings.GetOrCreateSettings().nodeXml))
        {
            var settings = RhTreeSettings.GetOrCreateSettings();
            styleSheets.Add(settings.nodeStyle);
            // Construct the node view from fed Node instance
            node = nodeRef;
            title = nodeRef.name.Replace("Node", "");
            viewDataKey = node.guid;
            style.left = node.position.x;
            style.top = node.position.y;
            tooltip = node.Tooltip;


            CreateInputPorts();
            CreateOutputPorts();

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
                input.style.flexDirection = FlexDirection.Column;
                input.viewDataKey = inputPort.GUID;
                inputContainer.Add(input);
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
    }
}