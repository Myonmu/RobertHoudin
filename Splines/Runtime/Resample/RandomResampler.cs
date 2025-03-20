using System;
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public class RandomResamplerEnumerator : SplineResampleEnumerator
    {
        private float _rngMin;
        private float _rngMax;
        public RandomResamplerEnumerator(float initialValue, float endValue, float rngMin, float rngMax): base(initialValue, endValue)
        {
            Assert.IsTrue(rngMin < rngMax, "Invalid random range");
            Assert.IsTrue(rngMin >= 0, "random range min value must be positive");
            
            _rngMin = rngMin;
            _rngMax = rngMax;
        }

        protected override float GetNextStep()
        {
            return Random.Range(_rngMin, _rngMax);
        }
    }
    
    [Serializable]
    public class RandomResampler: SplineResampler
    {
        public Vector2 randomRange;
        public int measurementAccuracy = 64;
        public override IEnumerator<float> GenerateResamplePoints(ISpline spline, float start = 0, float end = 1)
        {
            var splineLength = SplineUtils.MeasureLength(spline, start, end, measurementAccuracy);
            var min = randomRange.x / splineLength;
            var max = randomRange.y / splineLength;
            return new RandomResamplerEnumerator(start, end, min, max);
        }
    }
}