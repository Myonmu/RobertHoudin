using System;
using System.Collections.Generic;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    /// <summary>
    /// Generalized vector that may have 1 to 4 components.
    /// Doesn't allocate surplus memory.
    /// Convertible to any Unity Vector type.
    /// Getting/Setting non-existent components is allowed, but will always return 0 when getting
    /// or no effect when setting.
    /// </summary>
    [Serializable]
    public struct Vector
    {
        public Number[] components;

        public static Vector Alloc(int componentCount)
        {
            return new Vector()
            {
                components = new Number[componentCount]
            };
        }
        public Vector(Number x){components = new[]{x};}
        public Vector(Number x, Number y){components = new[]{x,y};}
        public Vector(Number x, Number y, Number z){components = new[]{x,y,z};}
        public Vector(Number x, Number y, Number z, Number w){components = new[]{x,y,z,w};}

        public Number x { get => this[0]; set => this[0] = value; }
        public Number y { get => this[1]; set => this[1] = value; }
        public Number z { get => this[2]; set => this[2] = value; }
        public Number w { get => this[3]; set => this[3] = value; }
        
        public int ComponentCount => components.Length;
        
        public Number this[int i]
        {
            get => i >= components.Length ? 0 : components[i];
            set {
                if (i >= components.Length) return;
                components[i] = value;
            }
        } 

        public static implicit operator Vector2(Vector v)
        {
            return new Vector2(v[0], v[1]);
        }

        public static implicit operator Vector(Vector2 v)
        {
            return new Vector(v.x, v.y);
        }

        public static implicit operator Vector3(Vector v)
        {
            return new Vector3(v[0], v[1], v[2]);
        }

        public static implicit operator Vector(Vector3 v)
        {
            return new Vector(v.x, v.y, v.z);
        }

        public static implicit operator Vector4(Vector v)
        {
            return  new Vector4(v[0], v[1], v[2], v[3]);
        }

        public static implicit operator Vector(Vector4 v)
        {
            return new Vector(v.x, v.y, v.z, v.w);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            for (var i = 0; i < Math.Max(a.ComponentCount, b.ComponentCount); i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            var aLarger = a.ComponentCount > b.ComponentCount;
            var result = aLarger ? a : b;
            for (var i = 0; i < (aLarger ? b.ComponentCount : a.ComponentCount); i++)
            {
                result[i] += aLarger ? b[i] : a[i];
            }
            return result;
        }

        public static Vector operator -(Vector a, Vector b)
        {
            var aLarger = a.ComponentCount > b.ComponentCount;
            var result = aLarger ? a : b;
            for (var i = 0;  i < (aLarger ? b.ComponentCount : a.ComponentCount); i++)
            {
                result[i] -= aLarger ? b[i] : a[i];
            }
            return result;
        }
    }
}