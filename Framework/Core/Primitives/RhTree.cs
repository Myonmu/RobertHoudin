using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RobertHoudin.Framework.Core.Primitives
{
    [CreateAssetMenu(fileName = "RhTree", menuName = "RobertHoudin/RH Tree")]
    public class RhTree : ScriptableObject
    {
        /// <summary>
        /// Used to provide context for data sources.
        /// But we only use the type info, not its values,
        /// so SerializeReference is slightly overkill.
        /// null is allowed as that indicates we fall back to draw
        /// a text box for binding instead.
        /// </summary>
        [SerializeReference]
        public IRhPropertyBlock propertyBlockType;
        public RhNode resultNode;

        [HideInInspector] public List<RhNode> nodes = new();

        //View Transform
        [HideInInspector] public Vector3 transformScale;
        [HideInInspector] public Vector3 transformPosition;

        //TODO: Incremental updates

        /// <summary>
        /// guid to Node
        /// </summary>
        private Dictionary<string, RhNode> _nodeCollection = new();
        /// <summary>
        /// guid to Port
        /// </summary>
        private Dictionary<string, RhPort> _portCollection = new();

        public void EvaluateTree(IRhPropertyBlock propertyBlock)
        {
            if (propertyBlockType != null && propertyBlock.GetType() != propertyBlockType.GetType())
            {
                throw new Exception($"Provided property block does not match the type {propertyBlockType.GetType().Name} expected by the RhTree.");
            }
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

        /// <summary>
        /// For debug only.
        /// Remove reference to nodes that no longer exist or not actually connected
        /// </summary>
        public void RemoveDanglingReferences()
        {
            ResetGuidCache();
            foreach (var port in _portCollection.Values)
            {
                port.GetConnectedPortGuids().RemoveAll(x => string.IsNullOrEmpty(x) || !_portCollection.ContainsKey(x));
            }
        }

        /// <summary>
        /// Whether a node can be traced to result node
        /// </summary>
        /// <returns></returns>
        public bool IsTraceableFromResultNode(RhNode node)
        {
            ResetTree(); //ensure node references are set
            return IsTraceableFromResultNodeRecursive(node);
            
            bool IsTraceableFromResultNodeRecursive(RhNode cursor)
            {
                if (cursor == null) return false;
                if (resultNode == null) return false;
                if (resultNode == cursor) return true;
                foreach (var output in cursor.OutputPortsGeneric)
                {
                    foreach (var connectedPort in output.GetConnectedPorts())
                    {
                        if (IsTraceableFromResultNodeRecursive(connectedPort.node)) return true;
                    }
                }
                return false;
            }
        }
    }
}