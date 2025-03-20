using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Quaternion
{
    public class QuaternionIdentity: RhNode
    {
        [RhOutputPort] public QuaternionPort iden;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            iden.SetValueNoBoxing(UnityEngine.Quaternion.identity);
            return true;
        }
    }
}