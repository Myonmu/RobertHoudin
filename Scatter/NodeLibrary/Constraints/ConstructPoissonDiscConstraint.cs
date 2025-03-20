using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.NodeLibrary.Scatterers;
using RobertHoudin.Scatter.Runtime;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.Constraints
{
    public class ConstructPoissonDiscConstraint: RhNode
    {
        [RhInputPort] public NumberBufferPort pointsPort;
        //[RhInputPort] public Vector2Port distancePort;
        [RhOutputPort] public PoissonDiskConstraintPort output;

        public DataSource<Vector2> distance;
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(new FastPoissonDiskGenerator.PoissonDiskConstraint()
            {
                existingPoints = pointsPort.GetValueNoBoxing(),
                //distance = distance.GetValue(context, this)
            });
            return true;
        }
    }
}