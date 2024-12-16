using System;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEditor;
using UnityEngine;
namespace RobertHoudin.Framework.Editor.Tree
{
    /// <summary>
    /// Tree editing operations.
    /// Migrated from RhTree.
    /// </summary>
    public class RhTreeEditing
    {
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

        public RhNode CreateNode(RhTree tree, Type type)
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

        public void DeleteNode(RhTree tree, RhNode node)
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