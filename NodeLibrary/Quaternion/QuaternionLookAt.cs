using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Quaternion
{
    public class QuaternionLookAt : RhNode
    {
        [RhInputPort] public Vector3PortDs forward;
        [RhInputPort] public Vector3PortDs up;
        [RhOutputPort] public QuaternionPort quat;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            try
            {
                var f = forward.GetValueNoBoxing();
                var u = up.GetValueNoBoxing();
                var result = UnityEngine.Quaternion.LookRotation(f, u);
                quat.SetValueNoBoxing(result);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }
    }
}