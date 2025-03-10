using System;
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using RobertHoudin.Utils.RuntimeCompatible;
namespace RobertHoudin.Splines.Runtime.RhSpline
{
    public class OrderedControlPointCollection
    {
        private float _accumulatedDistance;
        private List<ISplineControlPoint> _points = new();
        private List<float> _distances = new();
        public int Count => _points.Count;
        public void Update(IEnumerator<ISplineControlPoint> points)
        {
            _points.Clear();
            _distances.Clear();
            _accumulatedDistance = 0;
            ISplineControlPointWithPosition last = null;
            while (points.MoveNext())
            {
                var current = points.Current;
                if (current is not ISplineControlPointWithPosition cpWithPosition)
                    throw new Exception("All control points must implement ISplineWithPosition");
                if (last is not null)
                {
                    var diff = (cpWithPosition.Position - last.Position).magnitude;
                    _accumulatedDistance += diff;
                    _distances.Add(_accumulatedDistance);
                }
                else
                {
                    _distances.Add(0);
                }
                _points.Add(current);
                last = cpWithPosition;
            }
        }
        public OrderedControlPointCollection(IEnumerator<ISplineControlPoint> points)
        {
            Update(points);
        }

        public void SearchClosestControlPointsIndices(float t, out int before, out int after)
        {
            var actualDistance = ConvertToActualDistance(t);
            ClampSearch.FindClampingIndices(_distances, actualDistance, out before, out after);
        }

        /// <summary>
        /// Given normalized distance t, find the control points just before and after that distance.
        /// </summary>
        /// <param name="t">normalized distance (0 to 1), "eval point"</param>
        /// <param name="before">control point just before the eval point</param>
        /// <param name="after">control point just after the eval point</param>
        /// <returns>normalized distance between the before and after point</returns>
        public float GetAdjacentControlPoints(float t, out ISplineControlPoint before, out ISplineControlPoint after)
        {
            SearchClosestControlPointsIndices(t, out var beforeIndex, out var afterIndex);
            before = _points[beforeIndex];
            after = _points[afterIndex];
            return (t - _distances[beforeIndex]) / (_distances[afterIndex] - _distances[beforeIndex]);
        }

        public float GetDistance(int index)
        {
            return _distances[index];
        }

        public ISplineControlPoint GetControlPoint(int index)
        {
            return _points[index];
        }

        public float ConvertToActualDistance(float t)
        {
            return _accumulatedDistance * t;
        }
    }
}