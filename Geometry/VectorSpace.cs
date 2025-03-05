using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace RobertHoudin.Geometry
{
    public interface IVectorSpace
    {
    }
    public interface IVectorSpace<TVector, TScalar> : IVectorSpace
    {
        public TVector Sum(TVector a, TVector b);
        public TVector Scale(TVector a, TScalar s);
        public TVector Flip(TVector a);

        public TScalar Distance(TVector a, TVector b);
    }
    
    public interface IEuclideanVectorSpace<TVector>: IVectorSpace<TVector, float>{}

    public class EuclideanVectorSpaceT<T, TVector> where T : IEuclideanVectorSpace<TVector>, new()
    {
        public struct SegmentP : IParameterizedLineSegment<TVector, float>
        {
            public IVectorSpace<TVector, float> VectorSpace => VectorSpaceRegistry.Get<T>();
            public TVector A { get; set; }
            public TVector B { get; set; }
            public float Length => VectorSpace.Distance(A, B);
            public TVector Evaluate(float t)
            {
                return VectorSpace.Sum(A,
                    VectorSpace.Scale(
                        VectorSpace.Sum(B,
                            VectorSpace.Flip(A)
                        ),
                        t
                    )
                );
            }
        }
    }

    public class UnityEuclidean3 : EuclideanVectorSpaceT<UnityEuclidean3, Vector3>, IEuclideanVectorSpace<Vector3>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Sum(Vector3 a, Vector3 b)
        {
            return a + b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Scale(Vector3 a, float s)
        {
            return a * s;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Flip(Vector3 a)
        {
            return -a;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Distance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }
    }

    public class UnityEuclidean2 : EuclideanVectorSpaceT<UnityEuclidean2, Vector2>, IEuclideanVectorSpace<Vector2>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Sum(Vector2 a, Vector2 b)
        {
            return a + b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Scale(Vector2 a, float s)
        {
            return a * s;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Flip(Vector2 a)
        {
            return -a;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Distance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }
    }

    public static class VectorSpaceRegistry
    {
        private static readonly Dictionary<Type, IVectorSpace> VectorSpaces = new();
        public static T Get<T>() where T : IVectorSpace, new()
        {
            if (VectorSpaces.TryGetValue(typeof(T), out var space)) return (T)space;
            var vs = new T();
            VectorSpaces.Add(typeof(T), vs);
            return vs;
        }
    }
}