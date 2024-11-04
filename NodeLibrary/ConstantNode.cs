using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.MochiBTS.Core.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary
{
    public class ConstantNode : RhNode
    {
        public DataSource<int> value;
        public IntPort port;

        public override List<RhPort> OutputPorts => new(){port};

        protected override void OnBeginEvaluate(Agent agent, Blackboard blackboard)
        {
        }

        protected override bool OnEvaluate(Agent agent, Blackboard blackboard)
        {
            port.value = value.GetValue(agent, blackboard);
            return true;
        }
    }
}