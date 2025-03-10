using System;
using RobertHoudin.Geometry;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.RhSpline
{
    /// <summary>
    /// A spline rib is a segment perpendicular to the spline at a specific
    /// point on the spline.
    /// </summary>
    [Serializable]
    public class SplineRib : IParameterizedLineSegment<Vector3, float>
    {
        public IVectorSpace<Vector3, float> VectorSpace => VectorSpaceRegistry.Get<UnityEuclidean3>();
        public Vector3 referencePoint;
        public Vector3 splinePoint;
        public Vector3 direction;
        public Vector3 up;
        public float length;
        private Vector3 _extremityL;
        private Vector3 _extremityR;
        public float normalizedEvalPosition;
        /// <summary>
        /// how much to the left/right the rib segment is.
        /// when eccentricity is 0, the spline point is the mid-point of the segment.
        /// when eccentricity is -1, the rib is completely to the left of the spline point.
        /// when eccentricity is 1, the rib is completely to the right of the spline point.
        /// The interpolation is linear.
        /// </summary>
        public float eccentricity;

        public SplineRib(
            float normalizedEvalPosition,
            Vector3 referencePoint, 
            Vector3 splinePoint,
            Vector3 direction, 
            Vector3 up, 
            float length, 
            float eccentricity)
        {
            this.normalizedEvalPosition = normalizedEvalPosition;
            this.referencePoint = referencePoint;
            this.splinePoint = splinePoint;
            this.direction = direction;
            this.up = up;
            this.length = length;
            this.eccentricity = eccentricity;
            _extremityL = Vector3.zero;
            _extremityR = Vector3.zero;
            GetExtremities(out _extremityL, out _extremityR);
        }

        public Vector3 LeftVector => Vector3.Cross(up, direction).normalized;

        public Vector3 RightVector => -LeftVector;

        /// <summary>
        /// length to the left and right of the spline point.
        /// "left" is when viewing against the up vector, to the left of the direction vector.
        /// </summary>
        /// <returns>x: left length, y: right length</returns>
        public Vector2 GetSplitLength()
        {
            return new Vector2(length * (1 - eccentricity) / 2, length * (1 + eccentricity) / 2);
        }

        private void GetExtremities(out Vector3 left, out Vector3 right)
        {
            var splitLen = GetSplitLength();
            left = splinePoint + LeftVector * splitLen.x;
            right = splinePoint + RightVector * splitLen.y;
        }

        /// <summary>
        /// Pack the rib into 3 Vector4 for shader use
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void Pack(out Vector4 a, out Vector4 b, out Vector4 c)
        {
            var splitLen = GetSplitLength();
            a = new Vector4(splinePoint.x, splinePoint.y, splinePoint.z, eccentricity);
            b = new Vector4(direction.x, direction.y, direction.z, splitLen.x);
            c = new Vector4(up.x, up.y, up.z, splitLen.y);
        }

        public Vector3 A
        {
            get => _extremityL;
            set => throw new Exception("Extremities of a spline rib is readonly");
        }
        public Vector3 B
        {
            get => _extremityR;
            set => throw new Exception("Extremities of a spline rib is readonly");
        }

        public Vector3 AbsA => A + referencePoint;
        public Vector3 AbsB => B + referencePoint;

        public float LateralRotation
        {
            get {
                var l = LeftVector;
                var planar = l;
                planar.y = 0;
                return Vector3.Angle(planar, l);
            }
        }

        public Vector3 MidPoint => (_extremityL + _extremityR) / 2;
        public Vector3 AbsMidPoint => (AbsA + AbsB) / 2;
        public float Length => length;
        public Vector3 Evaluate(float t)
        {
            return Vector3.Lerp(A, B, t);
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(AbsA, AbsB);
        }
    }
}