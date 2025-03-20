using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
namespace RobertHoudin.NodeLibrary.Vector
{
    public class DecomposeVector: RhNode
    {
        [RhNodeData] public string swizzle = "xyzw";
        private VectorSubscript? _subscript;
        [RhInputPort] public VectorPortDs vector;
        [RhOutputPort] public NumberPort x;
        [RhOutputPort] public NumberPort y;
        [RhOutputPort] public NumberPort z;
        [RhOutputPort] public NumberPort w;

        public override void ResetNode(RhTree parent)
        {
            _subscript = null;
            base.ResetNode(parent);
        }
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            _subscript ??= new VectorSubscript(swizzle, vector.GetValueNoBoxing().ComponentCount);
            x.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript(_subscript.Value, 0));
            y.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript(_subscript.Value, 1));
            z.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript(_subscript.Value, 2));
            w.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript(_subscript.Value, 3));
            return true;
        }
    }
}