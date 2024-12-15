using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace RobertHoudin.Scatter.Scatterers
{
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
            public bool IsValid()
            {
                return existingPoints != null && existingPoints.Count > 0;
            }
        }

        private class Settings
        {
            public Vector2 bottomLeft;
            public Vector2 topRight;
            public Vector2 center;
            public Rect dimension;

            public Vector2 minimumDistance;
            public int iterationPerPoint;

            public float cellSize;
            public int gridWidth;
            public int gridHeight;
        }

        private class Bags
        {
            public Vector2?[,] grid;

            /// <summary>
            /// Result points
            /// </summary>
            public List<Vector2> samplePoints;

            /// <summary>
            /// Points that can be used to generate new points in the next iteration
            /// </summary>
            public List<Vector2> activePoints;
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
            var useConstraint = constraint is not null && constraint.IsValid();
            gen = new Random();
            var settings = BuildSettings(
                bottomLeft,
                topRight,
                minimumDistance,
                iterationPerPoint <= 0 ? DefaultIterationPerPoint : iterationPerPoint
            );

            var bags = new Bags()
            {
                grid = new Vector2?[settings.gridWidth + 1, settings.gridHeight + 1],
                samplePoints = new List<Vector2>(),
                activePoints = new List<Vector2>()
            };
            if (useConstraint)
            {
                bags.activePoints.AddRange(constraint.existingPoints);
            }
            else
            {
                GenerateFirstPoint(settings, bags);
            }

            do
            {
                var index = RandomRange(0, bags.activePoints.Count);

                var point = bags.activePoints[index];

                var found = false;
                Parallel.For(0, settings.iterationPerPoint,
                    i => { found |= GeneratePointAround(point, settings, bags, i, useConstraint); }
                );


                if (found == false)
                {
                    bags.activePoints.RemoveAt(index);
                }
            } while (bags.activePoints.Count > 0);

            return bags.samplePoints;
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
            if (bags.samplePoints.Count >= maxActivePointCount)
            {
                return false;
            }

            float minDistance = RandomRange(settings.minimumDistance.x, settings.minimumDistance.y);
            float maxDistance = 2f * minDistance;

            if (useConstraint)
            {
                maxDistance = minDistance + 3;
            }

            var p = GetRandPosInCircle(minDistance, maxDistance) + point;

            if (settings.dimension.Contains(p) == false)
            {
                return false;
            }

            foreach (var predicate in predicates)
            {
                if (!predicate(p)) return false;
            }


            var minimumSqr = minDistance * minDistance;
            var index = GetGridIndex(p, settings);

            var adjacentCellsRange = Vector2Int.FloorToInt(settings.minimumDistance / settings.cellSize);
            // clamping coordinates to cell range
            var fieldMin = Vector2Int.Max(Vector2Int.zero, index - adjacentCellsRange);
            var fieldMax = Vector2Int.Min(new Vector2Int(settings.gridWidth, settings.gridHeight),
                index + adjacentCellsRange);

            var drop = false;
            for (var i = fieldMin.x; i <= fieldMax.x && drop == false; i++)
            {
                for (var j = fieldMin.y; j <= fieldMax.y && drop == false; j++)
                {
                    var q = bags.grid[i, j];
                    if (q.HasValue && (q.Value - p).sqrMagnitude <= minimumSqr)
                    {
                        drop = true;
                    }
                }
            }

            if (drop) return false;
            var found = true;

            bags.samplePoints.Add(p);
            bags.grid[index.x, index.y] = p;

            if (!useConstraint)
            {
                bags.activePoints.Add(p);
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
                RandomRange(set.bottomLeft.x, set.topRight.x),
                RandomRange(set.bottomLeft.y, set.topRight.y)
            );

            var index = GetGridIndex(first, set);

            bags.grid[index.x, index.y] = first;
            bags.samplePoints.Add(first);
            bags.activePoints.Add(first);
        }

    #endregion

    #region "Utils"

        private static Vector2Int GetGridIndex(Vector2 point, Settings set)
        {
            return new Vector2Int(
                Mathf.FloorToInt((point.x - set.bottomLeft.x) / set.cellSize),
                Mathf.FloorToInt((point.y - set.bottomLeft.y) / set.cellSize)
            );
        }

        private static Settings BuildSettings(Vector2 bl, Vector2 tr, Vector2 min,
            int iteration)
        {
            var dimension = (tr - bl);
            var cell = min.x * InvertRootTwo;

            return new Settings()
            {
                bottomLeft = bl,
                topRight = tr,
                center = (bl + tr) * 0.5f,
                dimension = new Rect(new Vector2(bl.x, bl.y), new Vector2(dimension.x, dimension.y)),

                minimumDistance = min,
                iterationPerPoint = iteration,

                cellSize = cell,
                gridWidth = Mathf.CeilToInt(dimension.x / cell),
                gridHeight = Mathf.CeilToInt(dimension.y / cell)
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
}