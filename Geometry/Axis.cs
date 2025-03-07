using System;
using UnityEngine;
namespace RobertHoudin.MeshGeneration
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public static class AxisExtensions
    {
        public static Vector3 ToDirection(this Axis axis)
        {
            return axis switch
            {
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }

        public static Axis Next(this Axis axis)
        {
            return (Axis)(((int)axis + 1) % 3);
        }

        public static Axis Previous(this Axis axis)
        {
            return (Axis)(((int)axis + 2) % 3);
        }

        public static float ExtractThisComponentFromVector(this Axis axis, Vector3 vector)
        {
            return axis switch
            {
                Axis.X => vector.x,
                Axis.Y => vector.y,
                Axis.Z => vector.z,
                _ => vector.z
            };
        }
    }
    
}