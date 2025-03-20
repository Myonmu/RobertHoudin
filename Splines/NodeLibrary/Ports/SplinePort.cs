using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class SplinePort: RhSinglePort<ISpline>
    {
        
    }
    
    [Serializable]
    public class SplinePortDs : RhDataSourcePort<ISpline>
    {
        
    }
}