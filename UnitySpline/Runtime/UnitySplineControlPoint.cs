#if HAS_UNITY_SPLINES_PACKAGE
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
using UnityEngine.Splines;
namespace RobertHoudin.UnitySpline.Runtime
{
    public class UnitySplineControlPoint: IBasicSplineControlPoint, ISplineControlPointWithRotation
    {
        public BezierKnot knot;
        public Vector3 Position => knot.Position;
        public Vector3 Tangent => knot.TangentOut;
        public Quaternion Rotation => knot.Rotation;
    }
}
#endif