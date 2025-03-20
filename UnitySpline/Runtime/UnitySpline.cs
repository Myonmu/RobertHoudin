#if HAS_UNITY_SPLINES_PACKAGE
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
using UnityEngine.Splines;
namespace RobertHoudin.UnitySpline.Runtime
{
    public class UnitySpline: IBasicSpline
    {
        public SplineContainer container;
        public Spline spline;

        public UnitySpline(SplineContainer splineContainer, int index)
        {
            container = splineContainer;
            spline = splineContainer[index];
        }
        public static implicit operator Spline(UnitySpline spline)
        {
            return spline.spline;
        }
        
        public Vector3 EvaluatePosition(float t)
        {
            return spline.EvaluatePosition(t);
        }
        public Vector3 EvaluateTangent(float t)
        {
            return spline.EvaluateTangent(t);
        }
        public Vector3 EvaluateUp(float t)
        {
            return spline.EvaluateUpVector(t);
        }
        public Vector3 ReferencePoint => container.transform.position;
        public IEnumerator<ISplineControlPoint> GetControlPoints()
        {
            return new UnitySplineControlPointEnumerator(spline.Knots);
        }
        public int ControlPointCount => spline.Count;
    }
}
#endif