using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary.Vector
{
    public class ConstructVector3: RhNode
    {
        [RhInputPort] public NumberPortDs xPort;
        [RhInputPort] public NumberPortDs yPort;
        [RhInputPort] public NumberPortDs zPort;
        [RhOutputPort] public Vector3Port output;
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(new Vector3(
                xPort.GetValueNoBoxing(),
                yPort.GetValueNoBoxing(),
                zPort.GetValueNoBoxing()
                ));
            return true;
        }
    }
}