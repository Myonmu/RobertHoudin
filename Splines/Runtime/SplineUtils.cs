using System;
using System.Collections.Generic;
using RobertHoudin.Geometry;
using RobertHoudin.MeshGeneration;
using RobertHoudin.Splines.Runtime.Resample;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime
{
    public static class SplineUtils
    {
        public static SplineRib ExtractRib<T>(T spline, float t)
            where T : ISplineWithPosition, ISplineWithNormal, ISplineWithTangent, ISplineWithWidth
        {
            return new SplineRib(t,
                spline.ReferencePoint,
                spline.EvaluatePosition(t),
                spline.EvaluateTangent(t),
                spline.EvaluateUp(t),
                spline.EvaluateWidth(t),
                spline.EvaluateWidthEccentricity(t)
            );
        }

        public static SplineRib ExtractRib(ISpline spline, float t)
        {
            return new SplineRib(t,
                spline.ReferencePoint,
                (spline as ISplineWithPosition).EvaluatePosition(t),
                (spline as ISplineWithTangent).EvaluateTangent(t),
                (spline as ISplineWithNormal).EvaluateUp(t),
                (spline as ISplineWithWidth).EvaluateWidth(t),
                (spline as ISplineWithWidth).EvaluateWidthEccentricity(t)
            );
        }

        public static MeshBuffer GenerateMeshBufferFromRibs(IList<SplineRib> ribs, int divisions = 1)
        {
            if (ribs.Count < 2) throw new InvalidOperationException("Can't form any mesh with less than 2 ribs");
            if (divisions < 1) divisions = 1;
            // allocating buffers
            int vertexBufferSize = ribs.Count * (divisions + 1);
            int indexBufferSize = (ribs.Count - 1) * divisions * 2;
            var buffer = new MeshBuffer(vertexBufferSize, indexBufferSize);
            // write the first rib
            Geom.SplitLineSegmentNoAlloc(ribs[0], ref buffer.vertices, divisions: divisions);
            var lastRibVertexStart = 0;
            var indexWriteCursor = 0;
            for (var i = 1; i < ribs.Count; i++)
            {
                var writeStart = lastRibVertexStart + (divisions + 1);
                Geom.SplitLineSegmentNoAlloc(ribs[i], ref buffer.vertices,
                    divisions: divisions, writeStartIndex: writeStart);

                /*
                 *   prev. rib  0 --<---- 1 --------------
                 *              v   //    ^
                 *   curr. rib  2 ----->- 3 --------------
                 */
                for (var j = 0; j < divisions; j++)
                {
                    var v0 = lastRibVertexStart + j;
                    var v1 = lastRibVertexStart + j + 1;
                    var v2 = writeStart + j;
                    var v3 = writeStart + j + 1;
                    buffer.indices[indexWriteCursor++] = v0;
                    buffer.indices[indexWriteCursor++] = v2;
                    buffer.indices[indexWriteCursor++] = v1;
                    buffer.indices[indexWriteCursor++] = v3;
                    buffer.indices[indexWriteCursor++] = v1;
                    buffer.indices[indexWriteCursor++] = v2;
                }
                lastRibVertexStart = writeStart;
            }
            return buffer;
        }

        /// <summary>
        /// Generate a discrete spline by resampling a portion of the given spline.
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="resampler"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static RhDiscreteSpline GenerateDiscreteSpline(ISpline spline, ISplineResampler resampler, float start = 0, float end = 1)
        {
            var result = new RhDiscreteSpline();
            using var resamplePoints = resampler.GenerateResamplePoints(spline, start, end);
            while (resamplePoints.MoveNext())
            {
                var pos = resamplePoints.Current;
                result.PushPoint(new DiscreteControlPoint()
                {
                    Position = (spline as ISplineWithPosition)?.EvaluatePosition(pos) ?? Vector3.zero,
                    Tangent = (spline as ISplineWithTangent)?.EvaluateTangent(pos) ?? Vector3.zero,
                    Up = (spline as ISplineWithNormal)?.EvaluateUp(pos) ?? Vector3.zero,
                });
            }
            return result;
        }

        /// <summary>
        /// Measure the length of a spline by resampling
        /// </summary>
        /// <param name="spline">spline to measure</param>
        /// <param name="start">segment start</param>
        /// <param name="end">segment end</param>
        /// <param name="accuracy">resample count</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float MeasureLength(ISpline spline, float start, float end, float accuracy = 64)
        {
            if (spline is not ISplineWithPosition splineWithPos) throw new ArgumentException("spline doesn't implement ISplineWithPosition");
            var resampler = new UniformResampler();
            resampler.resampleFactor = accuracy;
            using var resamplePoints = resampler.GenerateResamplePoints(splineWithPos, start, end);
            var len = 0f;
            var isFirst = true;
            var last = Vector3.zero;
            while (resamplePoints.MoveNext())
            {
                var pos = resamplePoints.Current;
                if (isFirst)
                {
                    isFirst = false;
                    last = splineWithPos.EvaluatePosition(pos);
                    continue;
                }
                var currentPos = splineWithPos.EvaluatePosition(pos);
                len += (currentPos - last).magnitude;
                last = currentPos;
            }
            return len;
        }
    }
}