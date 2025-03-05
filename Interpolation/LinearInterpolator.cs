using System;
using System.Collections.Generic;
namespace RobertHoudin.Interpolation
{
    public class LinearInterpolatorF: IInterpolatorT<float>
    {
        public float Interpolate(float from, float to, float t)
        {
            return from + (to - from) * t;
        }
    }
}