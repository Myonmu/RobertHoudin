using UnityEngine;
namespace RobertHoudin.Geometry
{
    public interface IShape
    {
        public float Circumference { get; }
        public float Area { get; }
    }

    public interface IVolume
    {
        public float Surface { get; }
        public float Volume { get; }
    }
    public struct Circle: IShape
    {
        public Vector2 center;
        public float radius;
        public float Circumference => Mathf.PI * (radius + radius);
        public float Area => radius * radius * Mathf.PI;
    }

    public struct Sphere : IVolume
    {
        public Vector3 center;
        public float radius;
        public float Surface => 4 * Mathf.PI * radius * radius;
        public float Volume => 4 * Mathf.PI * radius * radius * radius / 3;
    }
}