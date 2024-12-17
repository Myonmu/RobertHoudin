using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Primitive
{
    public class NegateNode: RhNode
    {
        [RhInputPort] public BoolPortDs boolInput;
        [RhOutputPort] public BoolPort negateOutput;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            negateOutput.SetValueNoBoxing(!boolInput.GetValueNoBoxing());
            return true;
        }
    }
}