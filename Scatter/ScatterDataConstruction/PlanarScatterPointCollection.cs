using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    public class PointCollection2D
    {
        public List<Vector2> points = new();
    }

    public abstract class PlanarScatterer : RhNode
    {
        public PointCollection2DPort output = new();
        public override List<RhPort> OutputPorts => new() { output };
        
        public abstract PointCollection2D Scatter(RhExecutionContext context);
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(Scatter(context));
            return true;
        }
    }

    [Serializable] public class PointCollection2DPort : RhSinglePort<PointCollection2D> { }
}