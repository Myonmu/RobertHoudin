using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Numeric
{
    public class SumNumbers : RhNode

    {
        [RhInputPort] public MultiNumberPort inputs;
        [RhOutputPort] public NumberPort output;
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var sum = 0.0f;
            inputs.ForEachConnected(i => sum += i);
            output.SetValueNoBoxing(sum);
            return true;
        }
    }
}