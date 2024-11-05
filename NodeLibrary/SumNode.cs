using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.MochiBTS.Core.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary
{
    public class SumNode : RhNode

    {
        [RhInputPort] public MultiIntPort inputs;
        [RhOutputPort] public IntPort output;
        

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var sum = 0;
            inputs.ForEachConnected(i => sum += i);
            output.value = sum;
            return true;
        }
    }
}