using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class FastPoissonDiskGenerator
{
    public const float InvertRootTwo = 0.70710678118f; // Becaust two dimension grid.
    public const int DefaultIterationPerPoint = 10;
    public List<Vector2> curve = new List<Vector2>();
    private Random gen = new();
    public int maxActivePointCount = 100;

    #region "Structures"

    [Serializable]
    public class PoissonDiskConstraint
    {
        public Vector2 distance;
        public List<Vector2> points;
    }

    private class Settings
    {
        public Vector2 BottomLeft;
        public Vector2 TopRight;
        public Vector2 Center;
        public Rect Dimension;

        public Vector2 MinimumDistance;
        public Vector2 MinimumDistanceConstrained;
        public int IterationPerPoint;

        public float CellSize;
        public int GridWidth;
        public int GridHeight;
    }

    private class Bags
    {
        public Vector2?[,] Grid;
        public List<Vector2> SamplePoints;
        public List<Vector2> ActivePoints;
    }

    #endregion

    public void AddPoint(Vector2 p)
    {
        curve.Add(p);
    }

    private float RandomRange(float left, float right)
    {
        return ((float)gen.NextDouble()) * (right - left) + left;
    }

    private int RandomRange(int left, int right)
    {
        return gen.Next(left, right);
    }

    public List<Vector2> Sample(Vector2 bottomLeft, Vector2 topRight, Vector2 minimumDistance,
        int iterationPerPoint, PoissonDiskConstraint constraint)
    {
        var useConstraint = constraint is not null;
        gen = new Random();
        var settings = BuildSettings(
            bottomLeft,
            topRight,
            minimumDistance,
            constraint,
            iterationPerPoint <= 0 ? DefaultIterationPerPoint : iterationPerPoint
        );

        var bags = new Bags()
        {
            Grid = new Vector2?[settings.GridWidth + 1, settings.GridHeight + 1],
            SamplePoints = new List<Vector2>(),
            ActivePoints = new List<Vector2>()
        };
        if (useConstraint)
        {
            bags.ActivePoints.AddRange(constraint.points);
        }
        else
        {
            GetFirstPoint(settings, bags);
        }

        do
        {
            var index = RandomRange(0, bags.ActivePoints.Count);

            var point = bags.ActivePoints[index];

            var found = false;
            Parallel.For(0, settings.IterationPerPoint,
                i => { found |= GetNextPoint(point, settings, bags, i, useConstraint); }
            );


            if (found == false)
            {
                bags.ActivePoints.RemoveAt(index);
            }
        } while (bags.ActivePoints.Count > 0);

        return bags.SamplePoints;
    }

    #region "Algorithm Calculations"

    private bool GetNextPoint(Vector2 point, Settings set, Bags bags, int curIter, bool useConstraint = true)
    {
        if (bags.SamplePoints.Count >= maxActivePointCount)
        {
            return false;
        }

        float minimumOrigin = RandomRange(set.MinimumDistance.x, set.MinimumDistance.y);
        float minDistance = minimumOrigin;
        float maxDistance = 2f * minDistance;

        if (useConstraint)
        {
            minDistance = RandomRange(set.MinimumDistanceConstrained.x, set.MinimumDistanceConstrained.y);
            maxDistance = minDistance + 3;
        }

        var p = GetRandPosInCircle(minDistance, maxDistance) + point;

        if (set.Dimension.Contains(p) == false || !IsInPolygon(p, curve))
        {
            return false;
        }

        var minimum = minimumOrigin * minimumOrigin;
        var index = GetGridIndex(p, set);
        var drop = false;

        // Although it is Mathf.CeilToInt(set.MinimumDistance / set.CellSize) in the formula, It will be 2 after all.
        var around = 2;
        var fieldMin = new Vector2Int(Mathf.Max(0, index.x - around), Mathf.Max(0, index.y - around));
        var fieldMax = new Vector2Int(Mathf.Min(set.GridWidth, index.x + around),
            Mathf.Min(set.GridHeight, index.y + around));

        for (var i = fieldMin.x; i <= fieldMax.x && drop == false; i++)
        {
            for (var j = fieldMin.y; j <= fieldMax.y && drop == false; j++)
            {
                var q = bags.Grid[i, j];
                if (q.HasValue && (q.Value - p).sqrMagnitude <= minimum)
                {
                    drop = true;
                }
            }
        }

        if (drop) return false;
        var found = true;

        bags.SamplePoints.Add(p);
        bags.Grid[index.x, index.y] = p;

        if (!useConstraint)
        {
            bags.ActivePoints.Add(p);
        }
        else
        {
            found = false;
        }

        return found;
    }

    private void GetFirstPoint(Settings set, Bags bags)
    {
        var first = new Vector2(
            RandomRange(set.BottomLeft.x, set.TopRight.x),
            RandomRange(set.BottomLeft.y, set.TopRight.y)
        );

        var index = GetGridIndex(first, set);

        bags.Grid[index.x, index.y] = first;
        bags.SamplePoints.Add(first);
        bags.ActivePoints.Add(first);
    }

    #endregion

    #region "Utils"

    private static Vector2Int GetGridIndex(Vector2 point, Settings set)
    {
        return new Vector2Int(
            Mathf.FloorToInt((point.x - set.BottomLeft.x) / set.CellSize),
            Mathf.FloorToInt((point.y - set.BottomLeft.y) / set.CellSize)
        );
    }

    private static Settings BuildSettings(Vector2 bl, Vector2 tr, Vector2 min, PoissonDiskConstraint constraint,
        int iteration)
    {
        var dimension = (tr - bl);
        var cell = min.x * InvertRootTwo;

        return new Settings()
        {
            BottomLeft = bl,
            TopRight = tr,
            Center = (bl + tr) * 0.5f,
            Dimension = new Rect(new Vector2(bl.x, bl.y), new Vector2(dimension.x, dimension.y)),

            MinimumDistance = min,
            MinimumDistanceConstrained = constraint?.distance ?? min,
            IterationPerPoint = iteration,

            CellSize = cell,
            GridWidth = Mathf.CeilToInt(dimension.x / cell),
            GridHeight = Mathf.CeilToInt(dimension.y / cell)
        };
    }

    private Vector2 GetRandPosInCircle(float fieldMin, float fieldMax)
    {
        var theta = RandomRange(0, Mathf.PI * 2f);
        var radius = Mathf.Sqrt(RandomRange(fieldMax * fieldMax, fieldMin * fieldMin));

        return new Vector2(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta));
    }

    private static bool IsInPolygon(Vector2 p, IList<Vector2> vertices)
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

    #endregion
}