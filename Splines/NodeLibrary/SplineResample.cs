using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.Resample;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
namespace RobertHoudin.Splines.NodeLibrary
{
    public class SplineResample: RhNode
    {
        [RhNodeData] public bool useLocalSpace = false;
        [RhInputPort] public SplinePortDs spline;
        [RhInputPort] public SplineResamplerPortDs resampler;
        [RhInputPort] public NumberPortDs start;
        [RhInputPort] public NumberPortDs end;
        [RhOutputPort] public RhDiscreteSplinePort discreteSpline;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            if(spline.GetValueNoBoxing() is RhSpline rhSpline) rhSpline.OnBeginEvaluate();
            var sp = spline.GetValueNoBoxing();
            using var pts = resampler.GetValueNoBoxing()
                .GenerateResamplePoints(sp as ISpline, 
                    start.GetValueNoBoxing(), end.GetValueNoBoxing());
            var result = new RhDiscreteSpline();
            while (pts.MoveNext())
            {
                result.PushPoint(new DiscreteControlPoint()
                {
                    Position = ((sp as ISplineWithPosition).EvaluatePosition(pts.Current) 
                               + (useLocalSpace?Vector3.zero: sp.ReferencePoint)),
                    Tangent = (sp as ISplineWithTangent).EvaluateTangent(pts.Current),
                    Up = (sp as ISplineWithNormal).EvaluateUp(pts.Current)
                });
            }
            discreteSpline.SetValueNoBoxing(result);
            return true;
        }
    }
}