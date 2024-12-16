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
        public Action<RhNodeView> onNodeSelected;
        public Action<RhNodeView> onSetOutputFlag;
        private RhNodeSearchWindow searchWindow;
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
            searchWindow = ScriptableObject.CreateInstance<RhNodeSearchWindow>();
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

        public RhNodeView FindRhNodeView(RhNode node)
        {
            return FindNode(node) as RhNodeView;
        }

        /// <summary>
        /// Reevaluate if each node should be culled, and update their visuals
        /// </summary>
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
            // use hashset to prevent circular dependency
            var path = new HashSet<RhNode>();
            void UpdateCullingStatesRecursive(RhNode cursor)
            {
                if (cursor == null) return;
                var view = FindRhNodeView(cursor);
                view.isCulled = false;
                path.Add(cursor);
                foreach (var inputPort in cursor.InputPortsGeneric)
                {
                    if(!inputPort.IsActive) continue;
                    foreach (var connectedPort in inputPort.GetConnectedPorts())
                    {
                        if (path.Contains(connectedPort.node)) return;
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
            
            ValidatePorts();
        }

        public void ValidatePorts()
        {
            foreach (var node in tree.nodes)
            {
                foreach (var port in node.InputPortsGeneric)
                {
                    var view = FindPort(port) as RhPortView;
                    view?.Validate(port);
                }
            }
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            graphviewchange.elementsToRemove?.ForEach(e => {
                switch (e)
                {
                    case RhNodeView nodeView:
                        DeleteNode(nodeView.node);
                        break;
                    case Edge edge:
                        {
                            if (edge.output.node is RhNodeView parentView && edge.input.node is RhNodeView childView)
                            {
                                RhTreeEditing.RemoveConnection(
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
                if (edge.output.node is RhNodeView parentView && edge.input.node is RhNodeView childView)
                    RhTreeEditing.AddConnection(
                        parentView.node,
                        edge.output.viewDataKey,
                        childView.node,
                        edge.input.viewDataKey);
            });

            if (graphviewchange.movedElements is not null)
                nodes.ForEach(n => {
                    if (n is RhNodeView nodeView)
                        nodeView.SortChildren();
                });
            
            UpdateCullingStates();
            return graphviewchange;
        }

        private void CreateNodeView(RhNode node)
        {
            if (node == null) return;
            var nodeView = new RhNodeView(node, tree.resultNode == node)
            {
                onNodeSelected = onNodeSelected,
                onSetOutputFlag = onSetOutputFlag
            };
            AddElement(nodeView);
        }

        public void CreateNodeAtPosition(Type type, Vector2 position)
        {
            var node = CreateNode(type);
            node.position = position;
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
        public RhNode CreateNode(Type type)
        {
            var node = ScriptableObject.CreateInstance(type) as RhNode;
            node.name = type.Name;

            Undo.RecordObject(tree, "RH Tree (CreateNode)");
            tree.nodes.Add(node);

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, tree);
            Undo.RegisterCreatedObjectUndo(node, "RH Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }
        public void DeleteNode(RhNode node)
        {
            if (node is null) return;
            tree.ResetGuidCache();
            Undo.RecordObject(tree, "RH Tree (DeleteNode)");
            tree.nodes.Remove(node);
            node.DisconnectAll(tree);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }
    }
}