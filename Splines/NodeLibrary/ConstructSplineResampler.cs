using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.Resample;
using UnityEngine;
namespace RobertHoudin.Splines.NodeLibrary
{
    public class ConstructSplineResampler: RhNode
    {
        [SerializeReference] public ISplineResampler resampler;
        [RhOutputPort] public SplineResamplerPort result;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            result.SetValueNoBoxing(resampler);
            return true;
        }
    }
}