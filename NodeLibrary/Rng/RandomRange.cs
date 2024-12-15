using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary.Rng
{
    public class RandomRange: RhNode
    {
        [RhInputPort]public FloatPortDs minPort;
        [RhInputPort]public FloatPortDs maxPort;
        [RhOutputPort]public FloatPort result;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            result.SetValueNoBoxing(Random.Range(minPort.GetValueNoBoxing(), maxPort.GetValueNoBoxing()));
            return true;
        }
    }
}