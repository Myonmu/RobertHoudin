using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.ScatterDataConstruction;
using RobertHoudin.Scatter.Scatterers;
using UnityEngine;
namespace RobertHoudin.Scatter.Constraints
{
    public class ConstructPoissonDiscConstraint: RhNode
    {
        [RhInputPort] public PointCollection2DPort pointsPort;
        //[RhInputPort] public Vector2Port distancePort;
        [RhOutputPort] public PoissonDiskConstraintPort output;

        public DataSource<Vector2> distance;
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(new FastPoissonDiskGenerator.PoissonDiskConstraint()
            {
                existingPoints = pointsPort.GetValueNoBoxing()?.points,
                //distance = distance.GetValue(context, this)
            });
            return true;
        }
    }
}