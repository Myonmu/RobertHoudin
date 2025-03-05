using UnityEngine;
namespace RobertHoudin.Geometry
{
    public interface IParameterizedLineSegment<TPoint, TDistanceMetric>: ILineSegment<TPoint, TDistanceMetric>
    {
        public TPoint Evaluate(float t);
    }
}