using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary
{
    public class Sum2Node: RhNode
    {
        [RhInputPort] public IntPortDs a;
        [RhInputPort] public IntPortDs b;
        [RhOutputPort] public IntPort c;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            c.SetValueNoBoxing(a.GetValueNoBoxing() + b.GetValueNoBoxing());
            return true;
        }
    }
}