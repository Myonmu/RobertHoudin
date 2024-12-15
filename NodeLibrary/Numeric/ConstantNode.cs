using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary
{
    public class ConstantNode : RhNode
    {
        [RhInputPort] public NumberPortDs value;
        [RhOutputPort] public NumberPort port;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            port.SetValueNoBoxing(value.GetValueNoBoxing());
            return true;
        }
    }
}