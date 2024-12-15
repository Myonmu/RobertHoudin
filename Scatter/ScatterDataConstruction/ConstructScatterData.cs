using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.Scatter.ScatterDataConstruction
{
    public class ConstructScatterData : RhNode
    {
        [RhInputPort] public Vector3PortDs pos;
        [RhInputPort] public QuaternionPortDs rot;
        [RhInputPort] public Vector3PortDs scale;
        [RhOutputPort] public ScatterDataPort data;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            data.SetValueNoBoxing(new ScatterData()
            {
                pos = pos.GetValueNoBoxing(),
                rotation = rot.GetValueNoBoxing(),
                scale = scale.GetValueNoBoxing()
            });
            return true;
        }
    }
}