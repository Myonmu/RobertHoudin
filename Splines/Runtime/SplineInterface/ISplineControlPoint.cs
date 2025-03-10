using UnityEngine;
namespace RobertHoudin.Splines.Runtime.SplineInterface
{
    public interface ISplineControlPoint
    {
    }

    public interface ISplineControlPointWithPosition : ISplineControlPoint
    {
        public Vector3 Position { get; }
    }

    public interface ISplineControlPointWithRotation : ISplineControlPoint
    {
        public Quaternion Rotation { get; }
    }

    public interface ISplineControlPointWithScale : ISplineControlPoint
    {
        public Vector3 Scale { get; }
    }

    public interface ISplineControlPointWithTangent : ISplineControlPoint
    {
        public Vector3 Tangent { get; }
    }

    public interface ISplineControlPointWithNormal : ISplineControlPoint
    {
        public Vector3 Up { get; }
    }

    public interface IBasicSplineControlPoint:
        ISplineControlPointWithPosition,
        ISplineControlPointWithTangent
    {
    }

    public interface ISplineControlPointWithCustomData : ISplineControlPoint
    {
        public T GetCustomData<T>(int index);
    }
}