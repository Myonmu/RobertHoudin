﻿using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.Runtime;
namespace RobertHoudin.Scatter.NodeLibrary.Predicates
{
    public class InPolygonPredicate : RhNode
    {
        [RhInputPort] public Vector2CollectionPort input;
        [RhOutputPort] public Vector2PredicatesPort output;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            if (input.value == null) return false;
            output.SetValueNoBoxing((p) => 
                PolygonIntersection.IsInPolygon(p, input.GetValueNoBoxing()));
            return true;
        }
    }
}