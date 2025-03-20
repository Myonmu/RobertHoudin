using System.Collections.Generic;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.Scatterers
{
    public abstract class PlanarScatterer : RhNode
    {
        public NumberBufferPort output = new();
        public override List<RhPort> OutputPorts => new() { output };
        
        public abstract NumberBuffer Scatter(RhExecutionContext context);
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(Scatter(context));
            return true;
        }
    }
}