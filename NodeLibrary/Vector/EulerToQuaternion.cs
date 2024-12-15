using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.NodeLibrary.Vector
{
    public class EulerToQuaternion: RhNode
    {
        [RhInputPort] public Vector3PortDs input;
        [RhOutputPort] public QuaternionPort output;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(Quaternion.Euler(input.GetValueNoBoxing()));
            return true;
        }
    }
}