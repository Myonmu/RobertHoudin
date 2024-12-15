using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Editor.Node;
using RobertHoudin.Framework.Editor.Settings;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RobertHoudin.Framework.Editor.Tree
{
    #if UNITY_6000_0_OR_NEWER
    [UxmlElement]
    public partial class RhTreeView : GraphView
    #else
    public class RhTreeView : GraphView
    #endif
    {
        public Action<NodeView> onNodeSelected;
        public Action<NodeView> onSetOutputFlag;
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
            viewTransformChanged += _ => {
                if (tree is null) return;
                tree.transformPosition = viewTransform.position;
                tree.transformScale = viewTransform.scale; //Lost if recompiled...
            };
        }

        public void AddSearchWindow(RhTreeEditor.RhTreeEditor editor)
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
            tree.nodes.ForEach(n => {
                //connect output ports
                n.OutputPortsGeneric.ForEach(p => {
                    if (p is null) return;
                    var outputP = FindPort(p);
                    foreach (var connectedPort in p.GetConnectedPortGuids())
                    {
                        if (connectedPort is null) return;
                        var inputP = GetPortByGuid(connectedPort);
                        var edge = outputP.ConnectTo(inputP);
                        AddElement(edge);
                    }
                });
            });
            if (tree.transformScale == Vector3.zero) tree.transformScale = Vector3.one;
            UpdateViewTransform(tree.transformPosition, tree.transformScale);
            UpdateCullingStates();
        }

        public Port FindPort(RhPort port)
        {
            return GetPortByGuid(port.GUID);
        }

        public UnityEditor.Experimental.GraphView.Node FindNode(RhNode node)
        {
            return GetNodeByGuid(node.GUID);
        }

        public NodeView FindRhNodeView(RhNode node)
        {
            return FindNode(node) as NodeView;
        }

        public void UpdateCullingStates()
        {
            // set everything to culled
            foreach (var node in tree.nodes)
            {
                FindRhNodeView(node).isCulled = true;
            }
            if (tree.resultNode == null) return;
            // propagate non-culled state from results node
            // depth-first-search
            tree.ResetTree();
            void UpdateCullingStatesRecursive(RhNode cursor)
            {
                if (cursor == null) return;
                var view = FindRhNodeView(cursor);
                view.isCulled = false;
                foreach (var inputPort in cursor.InputPortsGeneric)
                {
                    if(!inputPort.IsActive) continue;
                    foreach (var connectedPort in inputPort.GetConnectedPorts())
                    {
                        UpdateCullingStatesRecursive(connectedPort.node);
                    }
                }
            }
            var cursor = tree.resultNode;
            UpdateCullingStatesRecursive(cursor);
            // update visuals
            foreach (var node in tree.nodes)
            {
                FindRhNodeView(node).UpdateCulledView();
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            graphviewchange.elementsToRemove?.ForEach(e => {
                switch (e)
                {
                    case NodeView nodeView:
                        tree.DeleteNode(nodeView.node);
                        break;
                    case Edge edge:
                        {
                            if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                            {
                                RhTree.RemoveConnection(
                                    parentView.node,
                                    edge.output.viewDataKey,
                                    childView.node,
                                    edge.input.viewDataKey);
                            }
                            break;
                        }
                }
            });

            graphviewchange.edgesToCreate?.ForEach(edge => {
                if (edge.output.node is NodeView parentView && edge.input.node is NodeView childView)
                    RhTree.AddConnection(
                        parentView.node,
                        edge.output.viewDataKey,
                        childView.node,
                        edge.input.viewDataKey);
            });

            if (graphviewchange.movedElements is not null)
                nodes.ForEach(n => {
                    if (n is NodeView nodeView)
                        nodeView.SortChildren();
                });
            
            UpdateCullingStates();
            return graphviewchange;
        }

        private void CreateNodeView(RhNode node)
        {
            if (node == null) return;
            var nodeView = new NodeView(node, tree.resultNode == node)
            {
                onNodeSelected = onNodeSelected,
                onSetOutputFlag = onSetOutputFlag
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
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<RhTreeView, UxmlTraits>
        {
        }
#endif
    }
}