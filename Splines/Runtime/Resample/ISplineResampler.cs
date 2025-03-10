using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public interface ISplineResampler
    {
        public IEnumerator<float> GenerateResamplePoints(ISpline spline, float start = 0, float end = 1);
        public List<float> GenerateResamplePointsCollection(ISpline spline, float start = 0, float end = 1);
    }
}