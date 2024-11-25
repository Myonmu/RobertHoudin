using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.Scatter
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