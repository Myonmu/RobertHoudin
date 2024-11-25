using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class FastPoissonDiskGenerator
{
    public const float InvertRootTwo = 0.70710678118f; // two dimension grid.
    public const int DefaultIterationPerPoint = 10;
    public List<Func<Vector2, bool>> predicates = new();
    private Random gen = new();
    public int maxActivePointCount = 100;

    #region "Structures"

    [Serializable]
    public class PoissonDiskConstraint
    {
        [FormerlySerializedAs("points")] public List<Vector2> existingPoints;
    }

    private class Settings
    {
        public Vector2 BottomLeft;
        public Vector2 TopRight;
        public Vector2 Center;
        public Rect Dimension;

        public Vector2 MinimumDistance;
        public int IterationPerPoint;

        public float CellSize;
        public int GridWidth;
        public int GridHeight;
    }

    private class Bags
    {
        public Vector2?[,] Grid;

        /// <summary>
        /// Result points
        /// </summary>
        public List<Vector2> SamplePoints;

        /// <summary>
        /// Points that can be used to generate new points in the next iteration
        /// </summary>
        public List<Vector2> ActivePoints;
    }

    #endregion

    private float RandomRange(float left, float right)
    {
        return ((float)gen.NextDouble()) * (right - left) + left;
    }

    private int RandomRange(int left, int right)
    {
        return gen.Next(left, right);
    }

    public List<Vector2> Sample(Bounds bounds, Vector2 minDist, int iterPerPoint, PoissonDiskConstraint constraint)
    {
        return Sample(new Vector2(bounds.min.x, bounds.min.z),
            new Vector2(bounds.max.x, bounds.max.z),
            minDist, iterPerPoint, constraint);
    }

    /// <summary>
    /// Circular/Square sample
    /// </summary>
    /// <param name="range"></param>
    /// <param name="minDist"></param>
    /// <param name="iterPerPoint"></param>
    /// <param name="constraint"></param>
    /// <returns></returns>
    public List<Vector2> Sample(float range, float minDist, int iterPerPoint, PoissonDiskConstraint constraint)
    {
        return Sample(
            new Vector2(-range, -range),
            new Vector2(range, range),
            new Vector2(minDist, minDist),
            iterPerPoint, constraint);
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
            bags.ActivePoints.AddRange(constraint.existingPoints);
        }
        else
        {
            GenerateFirstPoint(settings, bags);
        }

        do
        {
            var index = RandomRange(0, bags.ActivePoints.Count);

            var point = bags.ActivePoints[index];

            var found = false;
            Parallel.For(0, settings.IterationPerPoint,
                i => { found |= GeneratePointAround(point, settings, bags, i, useConstraint); }
            );


            if (found == false)
            {
                bags.ActivePoints.RemoveAt(index);
            }
        } while (bags.ActivePoints.Count > 0);

        return bags.SamplePoints;
    }

    #region "Algorithm Calculations"

    /// <summary>
    /// Tries to generate a point around the given point
    /// </summary>
    /// <param name="point"></param>
    /// <param name="settings"></param>
    /// <param name="bags"></param>
    /// <param name="curIter"></param>
    /// <param name="useConstraint"></param>
    /// <returns>whether the attempt succeeded</returns>
    private bool GeneratePointAround(Vector2 point, Settings settings, Bags bags, int curIter,
        bool useConstraint = true)
    {
        if (bags.SamplePoints.Count >= maxActivePointCount)
        {
            return false;
        }

        float minDistance = RandomRange(settings.MinimumDistance.x, settings.MinimumDistance.y);
        float maxDistance = 2f * minDistance;

        if (useConstraint)
        {
            maxDistance = minDistance + 3;
        }

        var p = GetRandPosInCircle(minDistance, maxDistance) + point;

        if (settings.Dimension.Contains(p) == false)
        {
            return false;
        }

        foreach (var predicate in predicates)
        {
            if (!predicate(p)) return false;
        }


        var minimumSqr = minDistance * minDistance;
        var index = GetGridIndex(p, settings);

        var adjacentCellsRange = Vector2Int.FloorToInt(settings.MinimumDistance / settings.CellSize);
        // clamping coordinates to cell range
        var fieldMin = Vector2Int.Max(Vector2Int.zero, index - adjacentCellsRange);
        var fieldMax = Vector2Int.Min(new Vector2Int(settings.GridWidth, settings.GridHeight),
            index + adjacentCellsRange);

        var drop = false;
        for (var i = fieldMin.x; i <= fieldMax.x && drop == false; i++)
        {
            for (var j = fieldMin.y; j <= fieldMax.y && drop == false; j++)
            {
                var q = bags.Grid[i, j];
                if (q.HasValue && (q.Value - p).sqrMagnitude <= minimumSqr)
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

    private void GenerateFirstPoint(Settings set, Bags bags)
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

    private static Settings BuildSettings(Vector2 bl, Vector2 tr, Vector2 min,
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

    public void AddPredicate(Func<Vector2, bool> predicate)
    {
        predicates.Add(predicate);
    }

    #endregion
}