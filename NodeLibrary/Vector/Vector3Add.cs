﻿using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.NodeLibrary.Vector
{
    public class Vector3Add: RhNode
    {
        [RhInputPort]public Vector3PortDs lhs;
        [RhInputPort]public Vector3PortDs rhs;
        [RhOutputPort] public Vector3Port output;
        
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(lhs.GetValueNoBoxing() + rhs.GetValueNoBoxing());
            return true;
        }
    }
}