using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Quaternion
{
    public class QuaternionMultiply: RhNode
    {
        [RhInputPort] public QuaternionPort lhs;
        [RhInputPort] public QuaternionPort rhs;
        [RhOutputPort] public QuaternionPort output;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(lhs.GetValueNoBoxing()*rhs.GetValueNoBoxing());
            return true;
        }
    }
}