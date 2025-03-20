using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.NodeLibrary;
using RobertHoudin.Splines.Runtime;
namespace RobertHoudin.Splines
{
    public class ConstructSplineRib : RhNode
    {
        [RhInputPort] public SplinePort spline;
        [RhInputPort] public NumberPortDs evalPosition;
        [RhOutputPort] public SplineRibPort splineRib;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            splineRib.SetValueNoBoxing(SplineUtils.ExtractRib(spline.GetValueNoBoxing(),  evalPosition.GetValueNoBoxing()));
            return true;
        }
    }
}