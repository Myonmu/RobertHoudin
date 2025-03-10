using System;
using System.Collections;
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public class UniformResamplerEnumerator : IEnumerator<float>
    {
        private float _initialValue;
        private bool _isInitialMove = true;
        private bool _prevMoveNextSucceeded = true;
        private float _increment;
        public UniformResamplerEnumerator(float initialValue, float increment)
        {
            _initialValue = initialValue;
            Current = initialValue;
            _increment = increment;
            _prevMoveNextSucceeded = true;
        }
        public bool MoveNext()
        {
            if (_isInitialMove)
            {
                _isInitialMove = false;
                return true;
            }
            Current += _increment;
            if ((Current > 1 || Mathf.Approximately(Current, 1)) && _prevMoveNextSucceeded)
            {
                Current = 0.99999999f;
                _prevMoveNextSucceeded = false;
                return true;
            }
            return _prevMoveNextSucceeded;
        }
        public void Reset()
        {
            _isInitialMove = true;
            _prevMoveNextSucceeded = true;
            Current = _initialValue;
        }
        public float Current
        {
            get;
            private set;
        }
        object IEnumerator.Current => Current;
        public void Dispose()
        {
        }
    }

    [Serializable]
    public class UniformResampler : ISplineResampler
    {
        public float resampleFactor = 100;
        public IEnumerator<float> GenerateResamplePoints(ISpline spline, float start, float end)
        {
            var increment = (end - start) / Mathf.Max(1, resampleFactor);
            return new UniformResamplerEnumerator(start, increment);
        }

        public List<float> GenerateResamplePointsCollection(ISpline spline, float start, float end)
        {
            using var resamplePoints = GenerateResamplePoints(spline, start, end);
            var result = new List<float>();
            while (resamplePoints.MoveNext())
            {
                result.Add(resamplePoints.Current);
            }
            return result;
        }
    }
}