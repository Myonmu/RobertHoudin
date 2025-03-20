using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.Resample;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class RhDiscreteSplinePort : RhSinglePort<RhDiscreteSpline>
    {
    }

    [Serializable]
    public class RhDiscreteSplinePortDs : RhDataSourcePort<RhDiscreteSpline>
    {
        
    }
}