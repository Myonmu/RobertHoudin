using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.NodeLibrary
{
    public class ExtractControlPointProperties: RhNode
    {
        [RhInputPort] public SplineControlPointPort controlPoint;
        [RhOutputPort] public Vector3Port controlPointPosition;
        [RhOutputPort] public Vector3Port controlPointTangent;
        [RhOutputPort] public Vector3Port controlPointNormal;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var cp = controlPoint.GetValueNoBoxing();
            if (cp is ISplineControlPointWithPosition cpWithPosition)
            {
                controlPointPosition.SetValueNoBoxing(cpWithPosition.Position);
            }
            if (cp is ISplineControlPointWithTangent cpWithTangent)
            {
                controlPointTangent.SetValueNoBoxing(cpWithTangent.Tangent);
            }
            if (cp is ISplineControlPointWithNormal cpWithNormal)
            {
                controlPointNormal.SetValueNoBoxing(cpWithNormal.Up);
            }
            return true;
        }
    }
}