using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using Sirenix.OdinInspector;
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
        public void EvaluateTree(RhPropertyBlock propertyBlock)
        {
             resultNode.EvaluateNode(new RhExecutionContext()
             {
                 propertyBlock = propertyBlock
             });
        }

        private static void Traverse(RhNode node, Action<RhNode> visitor)
        {
            if (node is null) return;
            visitor.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(n => Traverse(n, visitor));
        }

        public RhTree Clone()
        {

            var tree = Instantiate(this);
            tree.resultNode = tree.resultNode.Clone();
            tree.nodes = new List<RhNode>();
            Traverse(tree.resultNode, n => {
                if (n is null) return;
                tree.nodes.Add(n);
            });
            return tree;
        }

        public void ResetTree()
        {
            foreach (var node in nodes)
                node.ResetNode();
        }

        public void SetResultNode(RhNode node)
        {
            resultNode = node;
        }

        #if UNITY_EDITOR
        public RhNode CreateNode(Type type)
        {
            var node = CreateInstance(type) as RhNode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

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
            Undo.RecordObject(this, "RH Tree (DeleteNode)");
            nodes.Remove(node);
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
        public static List<RhNode> GetChildren(RhNode parent)
        {
            throw new NotImplementedException();
        }

        [Button]
        public void Test()
        {
            foreach (var node in nodes)
            {
                node.ResetNode();
            }
            resultNode.EvaluateNode(null);
            Debug.Log(resultNode.OutputPorts[0].GetValue());
        }

    }
}