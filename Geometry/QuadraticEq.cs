using System;
using UnityEngine;
namespace RobertHoudin.Geometry
{
    public struct QuadraticEq
    {
        public float a, b, c;

        public QuadraticEq(float a, float b, float c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public float Discriminant => b * b - 4 * a * c;

        /// <summary>
        /// Solve the quadratic equation, put the solutions in an array
        /// </summary>
        /// <param name="solutions"></param>
        /// <returns></returns>
        public int Solve(ref float[] solutions)
        {
            if (solutions.Length < 2) throw new ArgumentException("solution array is not enough for holding 2 solutions");
            var d = Discriminant;
            if (d < 0) return 0;
            if (Mathf.Approximately(d, 0))
            {
                solutions[0] = -b / (2 * a);
                return 1;
            }

            var sqrtD = Mathf.Sqrt(d);
            solutions[0] = (-b + sqrtD) / (2 * a);
            solutions[1] = (-b - sqrtD) / (2 * a);
            return 2;
        }
    }
}