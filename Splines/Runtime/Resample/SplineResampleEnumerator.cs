using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public abstract class SplineResampleEnumerator: IEnumerator<float>
    {
        private float _initialValue;
        private float _endValue;
        private bool _isInitialMove = true;
        private bool _prevMoveNextSucceeded = true;
        public SplineResampleEnumerator(float initialValue, float endValue)
        {
            Assert.IsTrue(initialValue < endValue, "Invalid sampling range");
            _initialValue = initialValue;
            _endValue = endValue;
            Current = initialValue;
            _prevMoveNextSucceeded = true;
        }

        protected abstract float GetNextStep();
        public bool MoveNext()
        {
            if (_isInitialMove)
            {
                _isInitialMove = false;
                return true;
            }
            Current += GetNextStep();
            if ((Current > _endValue || Mathf.Approximately(Current, _endValue)) && _prevMoveNextSucceeded)
            {
                Current = _endValue;
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
            protected set;
        }
        object IEnumerator.Current => Current;
        public void Dispose()
        {
        }
    }
}