using System.Collections.Generic;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.Runtime.Resample
{
    public abstract class SplineResampler: ISplineResampler
    {
        public abstract IEnumerator<float> GenerateResamplePoints(ISpline spline, float start = 0, float end = 1);
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