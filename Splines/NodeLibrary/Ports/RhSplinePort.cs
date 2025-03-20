using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.RhSpline;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class RhSplinePort: RhSinglePort<RhSpline>
    {
        
    }

    [Serializable]
    public class RhSplinePortDs : RhDataSourcePort<RhSpline>
    {
        
    }
}