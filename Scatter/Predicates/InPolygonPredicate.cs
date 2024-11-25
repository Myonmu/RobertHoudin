using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.Scatter
{
    public class InPolygonPredicate : RhNode
    {
        [RhInputPort] public PointCollection2DPort input;
        [RhOutputPort] public Point2DPredicatesPort output;

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            if (input.value == null) return false;
            output.SetValueNoBoxing((p) => 
                PolygonIntersection.IsInPolygon(p, input.GetValueNoBoxing().points));
            return true;
        }
    }
}