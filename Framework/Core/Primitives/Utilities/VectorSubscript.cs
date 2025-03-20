using System;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    public struct VectorSubscript
    {
        public int[] subscript;
        public int this[int i] => subscript[i];
        public VectorSubscript(string literal, int componentCount)
        {
            subscript = new int[4];
            for (int i = 0; i < 4; i++)
            {
                if (literal.Length <= i) subscript[i] = 0;
                else
                {
                    subscript[i] = literal[i] switch
                    {
                        'x' => 0,
                        'y' => 1,
                        'z' => 2,
                        'w' => 3,
                        'r' => 0,
                        'g' => 1,
                        'b' => 2,
                        'a' => 3,
                        _ => 0
                    };
                    subscript[i] = Math.Min(componentCount - 1, subscript[i]);
                }
            }
        } 
    }

    public static class VectorSubscriptExtensions
    {
        public static Vector4 Subscript4(this Vector vector, in VectorSubscript subscript)
        {
            return new Vector4(vector[subscript[0]], vector[subscript[1]], vector[subscript[2]], vector[subscript[3]]);
        }
        public static Vector3 Subscript3(this Vector vector, in VectorSubscript subscript)
        {
            return new Vector3(vector[subscript[0]], vector[subscript[1]], vector[subscript[2]]);
        }

        public static Vector2 Subscript2(this Vector vector, in VectorSubscript subscript)
        {
            return new Vector2(vector[subscript[0]], vector[subscript[1]]);
        }

        public static Number Subscript1(this Vector vector, in VectorSubscript subscript)
        {
            return vector[subscript[0]];
        }

        public static Number Subscript(this Vector vector, in VectorSubscript subscript, in int position)
        {
            return vector[subscript[position]];
        }
    }
}