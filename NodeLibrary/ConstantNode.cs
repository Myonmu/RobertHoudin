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
        [RhOutputPort] public IntPort port;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            port.value = value.GetValue(context, this);
            return true;
        }
    }
}