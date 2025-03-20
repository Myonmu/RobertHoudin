using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    /// <summary>
    /// Number type for reasonably small numbers (uses float under the hood)
    /// </summary>
    [Serializable]
    public struct Number
    {
        public float value;
        public static implicit operator int(Number number)
        {
            return (int)number.value;
        }

        public static implicit operator Number(bool value)
        {
            return new Number()
            {
                value = value ? 1 : 0
            };
        }

        public static implicit operator bool(Number number)
        {
            return number.value > 0;
        }

        public static implicit operator Number(int value)
        {
            return new Number()
            {
                value = value
            };
        }
        public static implicit operator float(Number number)
        {
            return number.value;
        }

        public static implicit operator Number(float value)
        {
            return new Number()
            {
                value = value
            };
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
        
        public static bool operator==(Number a, Number b)
        {
            return Mathf.Approximately(a.value, b.value);
        }
        public static bool operator !=(Number a, Number b)
        {
            return !(a == b);
        }

        public static Number operator +(Number a, Number b)
        {
            return a.value + b.value;
        }

        public static Number operator -(Number a, Number b)
        {
            return a.value - b.value;
        }
    }
}