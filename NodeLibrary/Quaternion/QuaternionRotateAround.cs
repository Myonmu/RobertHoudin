using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Vector
{
    public class QuaternionRotateAround: RhNode
    {
        [RhInputPort] public Vector3PortDs axis;
        [RhInputPort] public NumberPortDs angle;
        [RhOutputPort] public QuaternionPort rot;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            rot.SetValueNoBoxing(UnityEngine.Quaternion.AngleAxis(angle.GetValueNoBoxing(), axis.GetValueNoBoxing()));
            return true;
        }
    }
}