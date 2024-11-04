using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace MochiBTS.Editor
{
    public class RhTreeView : GraphView
    {
        public Action<NodeView> onNodeSelected;
        private NodeSearchWindow searchWindow;
        private readonly RhTreeSettings settings;
        public RhTree tree;

        public RhTreeView()
        {
            settings = RhTreeSettings.GetOrCreateSettings();
            if (settings.behaviorTreeStyle is null) return;

            Insert(0, new GridBackground());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            var styleSheet = settings.behaviorTreeStyle;
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
            viewTransformChanged += _ =>
            {
                if (tree is null) return;
                tree.transformPosition = viewTransform.position;
                tree.transformScale = viewTransform.scale; //Lost if recompiled...
            };
        }

        public void AddSearchWindow(RhTreeEditor editor)
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.targetView = this;
            searchWindow.targetWindow = editor;
            nodeCreationRequest = ctx =>
                SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), searchWindow);
        }

        private void OnUndoRedo()
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        public void PopulateView(RhTree treeParam)
        {
            tree = treeParam;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            tree.nodes.ForEach(CreateNodeView);
            tree.nodes.ForEach(n =>
            {
                //connect output ports
                n.OutputPorts.ForEach(p =>
                {
                    if (p is null) return;
                    var outputP = FindPort(p);
                    foreach (var connectedPort in p.GetConnectedPorts())
                    {
                        if (connectedPort is null) return;
                        var inputP = FindPort(connectedPort);
                        var edge = outputP.ConnectTo(inputP);
                        AddElement(edge);
                    }
                });
            });
            if (tree.transformScale == Vector3.zero) tree.transformScale = Vector3.one;
            UpdateViewTransform(tree.transformPosition, tree.transformScale);
        }

        private Port FindPort(RhPort port)
        {
            return GetPortByGuid(port.GUID);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            graphviewchange.elementsToRemove?.ForEach(e =>
            {
                switch (e)
                {
                    case NodeView nodeView:
                        tree.DeleteNode(nodeView.node);
                        break;
                    case Edge edge:
                    {
                        if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                            RhTree.RemoveConnection(
                                parentView.node,
                                edge.output.viewDataKey,
                                childView.node,
                                edge.input.viewDataKey);
                        break;
                    }
                }
            });

            graphviewchange.edgesToCreate?.ForEach(edge =>
            {
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                    RhTree.AddConnection(
                        parentView.node,
                        edge.output.viewDataKey,
                        childView.node,
                        edge.input.viewDataKey);
            });

            if (graphviewchange.movedElements is not null)
                nodes.ForEach(n =>
                {
                    if (n is NodeView nodeView)
                        nodeView.SortChildren();
                });
            return graphviewchange;
        }

        private void CreateNodeView(RhNode node)
        {
            var nodeView = new NodeView(node)
            {
                onNodeSelected = onNodeSelected
            };
            AddElement(nodeView);
        }

        /*[Obsolete]
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) //To be replaced by search window
        {
            //base.BuildContextualMenu(evt);
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}]{type.Name}",_=>CreateNode(type));
            }
            types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}]{type.Name}",_=>CreateNode(type));
            }
            types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types) {
                evt.menu.AppendAction($"[{type.BaseType.Name}]{type.Name}",_=>CreateNode(type));
            }
        }*/

        public void CreateNodeAtPosition(Type type, Vector2 position)
        {
            var node = tree.CreateNode(type);
            node.position = position;
            CreateNodeView(node);
        }

        public void CreateNode(Type type)
        {
            var node = tree.CreateNode(type);
            CreateNodeView(node);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()
                .Where(endPort => endPort.direction != startPort.direction &&
                                  endPort.node != startPort.node &&
                                  endPort.portType == startPort.portType)
                .ToList();
        }

        public new class UxmlFactory : UxmlFactory<RhTreeView, UxmlTraits>
        {
        }
    }
}