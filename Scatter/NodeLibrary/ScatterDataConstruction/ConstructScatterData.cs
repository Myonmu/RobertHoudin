using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.Runtime;
namespace RobertHoudin.Scatter.NodeLibrary.ScatterDataConstruction
{
    public class ConstructScatterData : RhNode
    {
        [RhInputPort] public BoolPortDs isDiscarded;
        [RhInputPort] public Vector3PortDs pos;
        [RhInputPort] public QuaternionPortDs rot;
        [RhInputPort] public Vector3PortDs scale;
        [RhInputPort] public Vector3PortDs normal;
        [RhInputPort] public IntPortDs objectId;
        [RhOutputPort] public ScatterDataPort data;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            data.SetValueNoBoxing(new ScatterData()
            {
                isDiscarded = isDiscarded.GetValueNoBoxing(),
                pos = pos.GetValueNoBoxing(),
                rotation = rot.GetValueNoBoxing(),
                scale = scale.GetValueNoBoxing(),
                normal = normal.GetValueNoBoxing(),
                objectId = objectId.GetValueNoBoxing(),
            });
            return true;
        }
    }
}