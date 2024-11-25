using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.NodeLibrary.Vector
{
    public class DecomposeVector2 : RhNode
    {
        [RhInputPort] public Vector2PortDs inputPort;
        [RhOutputPort] public FloatPort x;
        [RhOutputPort] public FloatPort y;
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var v2 = inputPort.GetValueNoBoxing();
            x.SetValueNoBoxing(v2.x);
            y.SetValueNoBoxing(v2.y);
            return true;
        }
    }
}