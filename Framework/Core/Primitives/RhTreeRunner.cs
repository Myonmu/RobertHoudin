﻿using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives
{
    public class RhTreeRunner : MonoBehaviour
    {
        public RhTree tree;
        [SerializeReference]
        public IRhPropertyBlock propertyBlock;
        public void Init()
        {
            ResetTree();
        }
        public void ResetTree()
        {
            tree.ResetTree();
        }
        [ContextMenu("Execute RhTree")]
        public void Execute()
        {
            ResetTree();
            tree.EvaluateTree(propertyBlock);
            Debug.Log(tree.resultNode.OutputPorts[0].GetValue());
        }
    }
}