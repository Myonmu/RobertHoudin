using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
using RobertHoudin.Utils.RuntimeCompatible;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.Resample
{
    /// <summary>
    /// Discrete spline only retains a list of points, usually the result of a resample operation
    /// </summary>
    public class RhDiscreteSpline: OrderedControlPointCollection, IBasicSpline
    {
        public RhDiscreteSpline(){}
        public RhDiscreteSpline(IEnumerator<ISplineControlPoint> points) : base(points)
        {
        }
        public Vector3 ReferencePoint { get; set; }
        public IEnumerator<ISplineControlPoint> GetControlPoints()
        {
            return points.GetEnumerator();
        }
        public int ControlPointCount => points.Count;
        
        public Vector3 EvaluateUp(float t)
        {
            var tt = GetAdjacentControlPoints(t, out var before, out var after);
            var a = (before as ISplineControlPointWithNormal).Up;
            var b = (after as ISplineControlPointWithNormal).Up;
            return Vector3.Lerp(a, b, tt);
        }
        public Vector3 EvaluateTangent(float t)
        {
            var tt = GetAdjacentControlPoints(t, out var before, out var after);
            var a = (before as ISplineControlPointWithTangent).Tangent;
            var b = (after as ISplineControlPointWithTangent).Tangent;
            return Vector3.Lerp(a, b, tt);
        }
        public Vector3 EvaluatePosition(float t)
        {
            var tt = GetAdjacentControlPoints(t, out var before, out var after);
            var a = (before as ISplineControlPointWithPosition).Position;
            var b = (after as ISplineControlPointWithPosition).Position;
            return Vector3.Lerp(a, b, tt);
        }
    }
}