using System;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Scatter.NodeLibrary.ObjectProviders;
using UnityEngine;
using Object = UnityEngine.Object;
namespace RobertHoudin.Sample.SimpleScatter
{
    [Serializable]
    public class SimpleScatterPropertyBlock: IRhPropertyBlock
    {
        public Transform rootTransform;
        public int maxActivePoints;
        public int k;
        public BoxCollider boxCollider;
        [HideInInspector] public Bounds bounds;
        public Vector2 distance;
        public WeightedObjectProvider objectProvider;
        public void OnBeginEvaluate()
        {
            if (rootTransform != null)
            {
                while (rootTransform.childCount > 0)
                {
                    Object.DestroyImmediate(rootTransform.GetChild(0).gameObject);
                }
            }
            bounds = boxCollider.bounds;
        }
    }
}