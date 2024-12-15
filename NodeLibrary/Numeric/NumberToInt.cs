using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary
{
    public class NumberToInt: RhNode
    {
        [RhInputPort] public NumberPort number;
        [RhOutputPort] public IntPort output;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(number.GetValueNoBoxing());
            return true;
        }
    }
}