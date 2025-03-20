using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.NodeLibrary
{
    public class DiscreteSplineToPointCollection : RhNode
    {
        [RhInputPort] public RhDiscreteSplinePortDs discreteSpline;
        [RhOutputPort] public NumberBufferPort positions;
        [RhOutputPort] public NumberBufferPort normals;
        [RhOutputPort] public NumberBufferPort tangents;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            positions.GetValueNoBoxing().Clear(3);
            normals.GetValueNoBoxing().Clear(3);
            tangents.GetValueNoBoxing().Clear(3);
            var spline = discreteSpline.GetValueNoBoxing();
            using var controlPoints = spline.GetControlPoints();
            try
            {
                while (controlPoints.MoveNext())
                {
                    if (positions.IsActive)
                    {
                        positions.GetValueNoBoxing().Add((controlPoints.Current as ISplineControlPointWithPosition).Position);
                    }
                    if (normals.IsActive)
                    {
                        normals.GetValueNoBoxing().Add((controlPoints.Current as ISplineControlPointWithNormal).Up);
                    }
                    if (tangents.IsActive)
                    {
                        tangents.GetValueNoBoxing().Add((controlPoints.Current as ISplineControlPointWithTangent).Tangent);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}