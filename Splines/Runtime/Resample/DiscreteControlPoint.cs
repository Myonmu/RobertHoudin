using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.RhSpline
{
    public class DiscreteControlPoint:
        ISplineControlPointWithPosition, 
        ISplineControlPointWithNormal, 
        ISplineControlPointWithTangent
    {

        public Vector3 Position
        {
            get;
            set;
        }
        public Vector3 Up
        {
            get;
            set;
        }
        public Vector3 Tangent
        {
            get;
            set;
        }
    }
}