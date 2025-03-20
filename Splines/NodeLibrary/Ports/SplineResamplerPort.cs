using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.Resample;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class SplineResamplerPort : RhSinglePort<ISplineResampler>
    {
    }

    [Serializable]
    public class SplineResamplerPortDs : RhDataSourcePort<ISplineResampler>
    {
        
    }
}