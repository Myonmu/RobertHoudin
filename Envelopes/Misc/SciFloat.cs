using System;
using UnityEngine;
namespace RobertHoudin.Envelopes.Misc
{
    [Serializable]
    public class SciFloat
    {
        public float value;
        public int power;
        public static implicit operator float(SciFloat sci)
        {
            return sci.value * Mathf.Pow(10, sci.power);
        }

        public static implicit operator SciFloat(float f)
        {
            return new SciFloat { value = f, power = 0};
        }
    }
}