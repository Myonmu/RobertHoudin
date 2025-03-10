using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.Resample
{
    /// <summary>
    /// Discrete spline only retains a list of points, usually the result of a resample operation
    /// </summary>
    public class RhDiscreteSpline: IBasicSpline
    {
        public float Length { get; private set; }
        private List<ISplineControlPoint> _points = new();
        public Vector3 ReferencePoint { get; set; }
        public void PushPoint(DiscreteControlPoint point)
        {
            var lastPoint = _points[^1] as DiscreteControlPoint;
            var distIncrement = (point.Position - lastPoint.Position).magnitude;
            Length += distIncrement;
            _points.Add(point); 
        }
        public IEnumerator<ISplineControlPoint> GetControlPoints()
        {
            return _points.GetEnumerator();
        }
        public Vector3 EvaluateUp(float t)
        {
            throw new System.NotImplementedException();
        }
        public Vector3 EvaluateTangent(float t)
        {
            throw new System.NotImplementedException();
        }
        public Vector3 EvaluatePosition(float t)
        {
            throw new System.NotImplementedException();
        }
    }
}