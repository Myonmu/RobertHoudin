using System.Collections.Generic;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.SplineInterface
{
    public interface ISpline
    {
        public Vector3 ReferencePoint { get; }
        public IEnumerator<ISplineControlPoint> GetControlPoints();
        public int ControlPointCount { get; }
    }
    public interface ISplineWithPosition: ISpline
    {
        public Vector3 EvaluatePosition(float t);
    }

    public interface ISplineWithTangent: ISpline
    {
        public Vector3 EvaluateTangent(float t);
    }

    public interface ISplineWithNormal: ISpline
    {
        public Vector3 EvaluateUp(float t);
    }

    /// <summary>
    /// "Width" is the length of a segment perpendicular to the spline's normal and tangent
    /// at the point of evaluation. aka. "lateral length", "bitangent length"
    /// </summary>
    /// <remarks>
    /// When you think in circle, "length" should be "diameter", not "radius"!
    /// </remarks>
    public interface ISplineWithWidth: ISpline
    {
        public float EvaluateWidth(float t);
        
        /// <summary>
        /// how off-center is the width segment.
        /// usually 0 means not off-center (mid-point is the point of evaluation),
        /// -1 means completely to the left of the tangent (bitangent direction).
        /// 1 means completely to the right of the tangent.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public float EvaluateWidthEccentricity(float t)
        {
            return 0;
        }
    }

    /// <summary>
    /// "Lateral Rotation" means rotation around the spline tangent, or "roll" 
    /// </summary>
    public interface ISplineWithLateralRotation : ISpline
    {
        public float EvaluateLateralRotation(float t);
    }


    /// <summary>
    /// A basic spline is a 3D curve that can evaluate to position, direction and normal
    /// </summary>
    public interface IBasicSpline : ISplineWithPosition, ISplineWithTangent, ISplineWithNormal
    {
    }

    public interface ISplineWithCustomData : ISpline
    {
        public T Evaluate<T>(string dataKey, float t);
    }
}