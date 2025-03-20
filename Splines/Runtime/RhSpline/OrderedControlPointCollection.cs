using System;
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using RobertHoudin.Utils.RuntimeCompatible;
namespace RobertHoudin.Splines.Runtime.RhSpline
{
    public class OrderedControlPointCollection
    {
        protected float accumulatedDistance;
        protected List<ISplineControlPoint> points = new();
        protected List<float> distances = new();
        public int Count => points.Count;
        public void Update(IEnumerator<ISplineControlPoint> points)
        {
            this.points.Clear();
            distances.Clear();
            accumulatedDistance = 0;
            while (points.MoveNext())
            {
                PushPoint(points.Current);
            }
        }
        public void PushPoint(ISplineControlPoint p)
        {
            if (p is not ISplineControlPointWithPosition cpWithPosition)
                throw new Exception("All control points must implement ISplineWithPosition");
            if (points.Count > 0)
            {
                var diff = (cpWithPosition.Position - (points[^1] as ISplineControlPointWithPosition).Position).magnitude;
                accumulatedDistance += diff;
                distances.Add(accumulatedDistance);
            }
            else
            {
                distances.Add(0);
            }
            points.Add(p);
        }
        public OrderedControlPointCollection(IEnumerator<ISplineControlPoint> points)
        {
            Update(points);
        }
        
        public OrderedControlPointCollection(){}

        public void SearchClosestControlPointsIndices(float t, out int before, out int after)
        {
            var actualDistance = ConvertToActualDistance(t);
            ClampSearch.FindClampingIndices(distances, actualDistance, out before, out after);
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
            before = points[beforeIndex];
            after = points[afterIndex];
            return (t - distances[beforeIndex]) / (distances[afterIndex] - distances[beforeIndex]);
        }

        public float GetDistance(int index)
        {
            return distances[index];
        }

        public ISplineControlPoint GetControlPoint(int index)
        {
            return points[index];
        }

        public float ConvertToActualDistance(float t)
        {
            return accumulatedDistance * t;
        }
    }
}