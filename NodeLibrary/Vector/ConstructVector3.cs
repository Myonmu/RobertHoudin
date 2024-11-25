using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary.Vector
{
    public class ConstructVector3: RhNode
    {
        [RhInputPort] public FloatPortDs xPort;
        [RhInputPort] public FloatPortDs yPort;
        [RhInputPort] public FloatPortDs zPort;
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