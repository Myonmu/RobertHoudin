using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.MochiBTS.Core.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary
{
    public class SumNode : RhNode

    {
        public MultiIntPort inputs;
        public IntPort output;

        public override List<RhPort> InputPorts => new() { inputs };

        public override List<RhPort> OutputPorts => new() { output };

        protected override void OnBeginEvaluate(Agent agent, Blackboard blackboard)
        {
        }

        protected override bool OnEvaluate(Agent agent, Blackboard blackboard)
        {
            var sum = 0;
            inputs.ForEachConnected(i => sum += i);
            output.value = sum;
            return true;
        }
    }
}