using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary.Rng
{
    public class RandomRange: RhNode
    {
        [RhInputPort]public NumberPortDs minPort;
        [RhInputPort]public NumberPortDs maxPort;
        [RhOutputPort]public NumberPortDs result;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            result.SetValueNoBoxing(Random.Range((float)minPort.GetValueNoBoxing(), maxPort.GetValueNoBoxing()));
            return true;
        }
    }
}