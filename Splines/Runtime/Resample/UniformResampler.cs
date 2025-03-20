using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public class UniformResamplerEnumerator : SplineResampleEnumerator
    {
        private float _increment;
        public UniformResamplerEnumerator(float initialValue, float endValue, float increment):base(initialValue, endValue)
        {
            Assert.IsTrue(increment > 0 , "Increment must be greater than zero");
            _increment = increment;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override float GetNextStep()
        {
            return _increment;
        }
    }

    [Serializable]
    public class UniformResampler : SplineResampler
    {
        public float resampleFactor = 100;
        public override IEnumerator<float> GenerateResamplePoints(ISpline spline, float start = 0, float end = 1)
        {
            var increment = (end - start) / Mathf.Max(1, resampleFactor);
            return new UniformResamplerEnumerator(start, end, increment);
        }
    }
}