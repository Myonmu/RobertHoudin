using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
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
        public Blackboard blackboard;
        //View Transform
        [HideInInspector] public Vector3 transformScale;
        [HideInInspector] public Vector3 transformPosition;
        public void EvaluateTree(Agent agent, bool forceExecute = false)
        {
             resultNode.EvaluateNode(agent, blackboard);
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
            if (tree.blackboard is not null) tree.blackboard = blackboard.Clone();
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

        public static void AddChild(RhNode parent, RhNode child)
        {
            if (parent is null || child is null) return;
            Undo.RecordObject(parent, "RH Tree (AddChild)");
            
            child.AssignParent(parent);
            EditorUtility.SetDirty(parent);

        }

        public static void RemoveChild(RhNode parent, RhNode child)
        {
            if (parent is null || child is null) return;
            Undo.RecordObject(parent, "RH Tree (RemoveChild)");
            
            child.AssignParent(null);
            EditorUtility.SetDirty(parent);

        }



        #endif
        public static List<RhNode> GetChildren(RhNode parent)
        {
            throw new NotImplementedException();
        }

    }
}