using System;
using System.Collections.Generic;
using UnityEngine;
namespace RobertHoudin.Geometry
{
    public static class Geom
    {
        public static void SplitLineSegmentNoAlloc<TPoint, TDistanceMetric>(
            IParameterizedLineSegment<TPoint, TDistanceMetric> segment,
            ref TPoint[] points, int writeStartIndex = 0, int divisions = 1)
        {
            if (points == null || points.Length <= writeStartIndex + divisions + 1)
            {
                throw new Exception("Points array is null or has no enough space");
            }
            var a = segment.A;
            var b = segment.B;

            var vs = segment.VectorSpace;
            if (vs == null) throw new Exception("VectorSpace is null");
            var sum = vs.Sum(a, b);
            var cursor = writeStartIndex;
            for (int i = 0; i <= divisions; i++)
            {
                points[cursor] = segment.Evaluate(i / (float)divisions);
                cursor++;
            }
        }


        public static Bounds CalculateMeshRendererBounds(GameObject gameObject, bool worldSpace = false)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                min = Vector3.Min(renderer.bounds.min, min);
                max = Vector3.Max(renderer.bounds.max, max);
            }
            var result = new Bounds((min + max) / 2, (max - min));
            if (!worldSpace)
            {
                result.center -= gameObject.transform.position;
            }
            return result;
        }

        public static Bounds CalculatePointCollectionBounds(List<Vector3> points)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var point in points)
            {
                min = Vector3.Min(point, min);
                max = Vector3.Max(point, max);
            }
            return new Bounds((min + max) / 2, (max - min));
        }

        public static Bounds CalculatePointCollectionBounds(params Vector3[] points)
        {
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            foreach (var point in points)
            {
                min = Vector3.Min(point, min);
                max = Vector3.Max(point, max);
            }
            return new Bounds((min + max) / 2, (max - min));
        }
    }
}