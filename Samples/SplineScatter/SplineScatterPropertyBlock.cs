using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Scatter.NodeLibrary.ObjectProviders;
using RobertHoudin.Scatter.Runtime;
using RobertHoudin.Splines.Runtime.Resample;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Samples.SplineScatter
{
    public class SplineScatterPropertyBlock: IRhPropertyBlock
    {
        [SerializeReference] public ISpline spline;
        [SerializeReference] public ISplineResampler resampler;
        public bool randomYRot;
        public float randomYRotMin;
        public float randomYRotMax;
        public float constYRot;
        public bool lookAtSplineDir;
        public bool randomScale;
        public float randomScaleMin;
        public float randomScaleMax;
        public float constScale;
        public WeightedObjectProvider objectProvider;
        public IObjectProvider _objectProvider;
        public Transform spawnRoot;
        
        public void OnBeginEvaluate()
        {
            if (spawnRoot != null)
            {
                while (spawnRoot.childCount > 0)
                {
                    Object.DestroyImmediate(spawnRoot.GetChild(0).gameObject);
                }
            }
            _objectProvider = objectProvider;
        }
    }
}