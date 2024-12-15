using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.ScatterDataConstruction;
namespace RobertHoudin.Scatter.Predicates
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