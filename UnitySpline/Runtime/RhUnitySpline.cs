#if HAS_UNITY_SPLINES_PACKAGE
using System;
using RobertHoudin.Splines.Runtime.RhSpline;
using UnityEngine.Splines;
namespace RobertHoudin.UnitySpline.Runtime
{
    [Serializable]
    public class RhUnitySpline: RhSpline<UnitySpline>
    {
        public SplineContainer unitySplineContainer;
        public int splineIndex;

        public override void OnBeginEvaluate()
        {
            rawSpline = new UnitySpline(unitySplineContainer, splineIndex);
        }
    }
}
#endif