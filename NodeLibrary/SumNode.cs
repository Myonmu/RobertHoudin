using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary
{
    public class SumNode : RhNode

    {
        [RhInputPort] public MultiIntPort inputs;
        [RhOutputPort] public IntPort output;
        

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var sum = 0;
            inputs.ForEachConnected(i => sum += i);
            output.SetValueNoBoxing(sum);
            return true;
        }
    }
}