#if HAS_UNITY_SPLINES_PACKAGE
using System.Collections;
using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine.Splines;
namespace RobertHoudin.UnitySpline.Runtime
{
    public class UnitySplineControlPointEnumerator: IEnumerator<ISplineControlPoint>
    {
        private IEnumerator<BezierKnot> _rawEnumerator;
        public UnitySplineControlPointEnumerator(IEnumerable<BezierKnot> rawPoints)
        {
            _rawEnumerator = rawPoints.GetEnumerator();
        }
        public bool MoveNext()
        {
            var success = _rawEnumerator.MoveNext();
            if (!success) return false;
            Current = new UnitySplineControlPoint(){knot = _rawEnumerator.Current};
            return true;
        }
        public void Reset()
        {
        }
        public ISplineControlPoint Current { get; private set; }
        
        object IEnumerator.Current => Current;
        public void Dispose()
        {
            _rawEnumerator.Dispose();
        }
    }
}
#endif