using System.Collections.Generic;
using UnityEngine;
namespace RobertHoudin.Scatter.Runtime
{
    public static class PolygonIntersection
    {
        public static bool IsInPolygon(Vector2 p, IList<Vector2> vertices)
        {
            var count = 0;
            var previous = vertices.Count - 1;
            for (var i = 0; i < vertices.Count; i++)
            {
                var a = vertices[previous];
                var b = vertices[i];
                previous = i;
                if (CheckRightwardRaycastIntersect(p, a, b)) count++;
            }

            return count % 2 == 1;
        }

        private static bool CheckRightwardRaycastIntersect(Vector2 source, Vector2 a, Vector2 b)
        {
            // when assuming segments are ordered, we discard intersection with a to avoid double counting
            if (Mathf.Approximately(source.y, a.y)) return false;

            float minY = Mathf.Min(a.y, b.y);
            float maxY = Mathf.Max(a.y, b.y);
            // source not with y range, the raycast cannot hit
            // also account for grazing angles (not counted as intersection)
            if (source.y > maxY || source.y < minY) return false;

            // see if the source point is actually on the left of the segment
            float minX = Mathf.Min(a.x, b.x);
            float maxX = Mathf.Max(a.x, b.x);

            if (source.x < minX) return true;
            if (source.x > maxX) return false;

            // when the source point is in the bounding box of the segment:

            // Calculate the intersection point's x-coordinate using the line equation
            float slope = (b.y - a.y) / (b.x - a.x);
            float intersectX = a.x + (source.y - a.y) / slope; // Calculate x at the source.y level

            // The ray intersects the segment if the intersecting x is greater than the source.x
            return intersectX > source.x;
        }
    }
}