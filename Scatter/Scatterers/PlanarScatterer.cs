using System.Collections.Generic;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Scatter.ScatterDataConstruction
{
    public abstract class PlanarScatterer : RhNode
    {
        public Vector2CollectionPort output = new();
        public override List<RhPort> OutputPorts => new() { output };
        
        public abstract List<Vector2> Scatter(RhExecutionContext context);
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(Scatter(context));
            return true;
        }
    }
}