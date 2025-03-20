using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class SplineControlPointPort: RhSinglePort<ISplineControlPoint>
    {
        
    }

    [Serializable]
    public class SplineControlPointPortDs : RhDataSourcePort<ISplineControlPoint>
    {
        
    }
}