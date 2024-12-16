using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Raycasting
{
    public class RaycastNode : RhNode
    {
        [RhInputPort] public Vector3PortDs start;
        [RhInputPort] public Vector3PortDs direction;
        [RhInputPort] public FloatPortDs maxDistance;
        [RhInputPort] public LayerMaskPortDs layerMask;
        
        [RhOutputPort] public BoolPort isHit;
        [RhOutputPort] public Vector3Port hitPosition;
        [RhOutputPort] public NumberPort hitDistance;
        [RhOutputPort] public Vector3Port hitNormal;
        [RhOutputPort] public TransformPort hitTransform;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var hit = Physics.Raycast(start.GetValueNoBoxing(), direction.GetValueNoBoxing(),
                out var raycastHit,
                 maxDistance.GetValueNoBoxing(), layerMask.GetValueNoBoxing());
            isHit.SetValueNoBoxing(hit);
            hitPosition.SetValueNoBoxing(raycastHit.point);
            hitDistance.SetValueNoBoxing(raycastHit.distance);
            hitNormal.SetValueNoBoxing(raycastHit.normal);
            hitTransform.SetValueNoBoxing(raycastHit.transform);
            return true;
        }
    }
}