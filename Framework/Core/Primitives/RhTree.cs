using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace RobertHoudin.Framework.Core.Primitives
{
    [CreateAssetMenu(fileName = "RhTree", menuName = "RobertHoudin/RH Tree")]
    public class RhTree : ScriptableObject
    {
        public RhNode resultNode;

        [HideInInspector] public List<RhNode> nodes = new();

        //View Transform
        [HideInInspector] public Vector3 transformScale;
        [HideInInspector] public Vector3 transformPosition;

        //TODO: Incremental updates
        private Dictionary<string, RhNode> _nodeCollection = new();
        private Dictionary<string, RhPort> _portCollection = new();

        public void EvaluateTree(RhPropertyBlock propertyBlock)
        {
            resultNode.EvaluateNode(new RhExecutionContext()
            {
                propertyBlock = propertyBlock
            });
        }

        /// <summary>
        /// Reconstructs guid cache. 
        /// </summary>
        public void ResetGuidCache()
        {
            _portCollection.Clear();
            _nodeCollection.Clear();
            foreach (var node in nodes)
            {
                _nodeCollection.TryAdd(node.GUID, node);
                foreach (var port in node.InputPortsGeneric)
                {
                    _portCollection.TryAdd(port.GUID, port);
                }

                foreach (var port in node.OutputPortsGeneric)
                {
                    _portCollection.TryAdd(port.GUID, port);
                }
            }
        }

        /// <summary>
        /// Reset nodes and propagate runtime references to nodes and ports
        /// </summary>
        public void ResetTree()
        {
            foreach (var node in nodes)
                node.ResetNode(this);
            ResetGuidCache();
        }

        public RhNode SetResultNode(RhNode node)
        {
            var prevResultNode = resultNode;
            resultNode = node;
            return prevResultNode;
        }

#if UNITY_EDITOR
        public RhNode CreateNode(Type type)
        {
            var node = CreateInstance(type) as RhNode;
            node.name = type.Name;

            Undo.RecordObject(this, "RH Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying)
                AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "RH Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(RhNode node)
        {
            if (node is null) return;
            ResetGuidCache();
            Undo.RecordObject(this, "RH Tree (DeleteNode)");
            nodes.Remove(node);
            node.DisconnectAll(this);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public static void AddConnection(RhNode from, string fromPortGuid, RhNode to, string toPortGuid)
        {
            if (from is null || to is null) return;
            var fromPort = from.GetOutputPortByGUID(fromPortGuid);
            var toPort = to.GetInputPortByGUID(toPortGuid);
            if (fromPort is null || toPort is null) return;

            Undo.RecordObject(from, "RH Tree (Connect)");
            fromPort.Connect(toPort);
            toPort.Connect(fromPort);
            EditorUtility.SetDirty(from);
        }

        public static void RemoveConnection(RhNode from, string fromPortGuid, RhNode to, string toPortGuid)
        {
            if (from is null || to is null) return;
            var fromPort = from.GetOutputPortByGUID(fromPortGuid);
            var toPort = to.GetInputPortByGUID(toPortGuid);
            if (fromPort is null || toPort is null) return;

            Undo.RecordObject(from, "RH Tree (Disconnect)");
            fromPort.Disconnect(toPort);
            toPort.Disconnect(fromPort);
            EditorUtility.SetDirty(from);
        }

#endif

        /// <summary>
        /// Finds a node by guid from guid cache (guid cache must have been initialized)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public RhNode FindNodeByGUID(string guid)
        {
            return _nodeCollection.GetValueOrDefault(guid);
        }

        /// <summary>
        /// Finds a port by guid from guid cache (guid cache must have been initialized)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public RhPort FindPortByGUID(string guid)
        {
            return _portCollection.GetValueOrDefault(guid);
        }

        public void RemoveDanglingReferences()
        {
            ResetGuidCache();
            foreach (var port in _portCollection.Values)
            {
                port.GetConnectedPortGuids().RemoveAll(x => string.IsNullOrEmpty(x) || !_portCollection.ContainsKey(x));
            }
        }
        
        public void Test()
        {
            ResetTree();
            resultNode.EvaluateNode(null);
            var val = resultNode.OutputPorts[0].GetValue();
            Debug.Log(val);
        }
    }
}