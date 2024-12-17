using System;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Scatter;
using RobertHoudin.Scatter.ObjectProviders;
using UnityEngine;
namespace RobertHoudin.Sample.SimpleScatter
{
    [Serializable]
    public class SimpleScatterPropertyBlock: IRhPropertyBlock
    {
        public Transform rootTransform;
        public int maxActivePoints;
        public int k;
        [HideInInspector] public Bounds bounds;
        public Vector2 distance;
        public WeightedObjectProvider objectProvider;
        public void OnBeginEvaluate()
        {
            var boxCollider = rootTransform.gameObject.GetComponent<BoxCollider>();
            bounds = boxCollider.bounds;
        }
    }
}