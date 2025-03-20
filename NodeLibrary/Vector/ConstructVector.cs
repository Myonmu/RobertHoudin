using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Vector
{
    public class ConstructVector: RhNode
    {
        [RhInputPort] public NumberPortDs x;
        [RhInputPort] public NumberPortDs y;
        [RhInputPort] public NumberPortDs z;
        [RhInputPort] public NumberPortDs w;
        [RhOutputPort] public VectorPort vector;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var allocCount = 0;
            if (x.IsActive) allocCount++;
            if (y.IsActive) allocCount++;
            if (z.IsActive) allocCount++;
            if (w.IsActive) allocCount++;
            var v = Framework.Core.Primitives.Utilities.Vector.Alloc(allocCount);
            v.x = x.GetValueNoBoxing();
            v.y = y.GetValueNoBoxing();
            v.z = z.GetValueNoBoxing();
            v.w = w.GetValueNoBoxing();
            vector.SetValueNoBoxing(v);
            return true;
        }
    }
}