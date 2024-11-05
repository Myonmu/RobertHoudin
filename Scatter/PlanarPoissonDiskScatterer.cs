using System;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.MochiBTS.Core.Ports;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    [Serializable]
    public class PoissonDiskConstraintPort : RhSinglePort<FastPoissonDiskGenerator.PoissonDiskConstraint>
    {
    }

    public class PlanarPoissonDiskScatterer : PlanarScatterer
    {
        [RhInputPort] public BoundsPort boundsPort;
        [RhInputPort] public IntPort kPort;
        [RhInputPort] public Vector2Port distancePort;
        [RhInputPort] public PoissonDiskConstraintPort constraintsPort;
        

        public DataSource<Bounds> bounds = new();
        public DataSource<int> k = new();
        public DataSource<Vector2> distance = new();
        public DataSource<FastPoissonDiskGenerator.PoissonDiskConstraint> constraints = new();
        
        
        public override PointCollection2D Scatter(RhExecutionContext context)
        {
            var gen = new FastPoissonDiskGenerator();
            var b = bounds.GetValue(context, this);
            var points = gen.Sample(new Vector2(b.min.x, b.min.z),
                new Vector2(b.max.x, b.max.z), distance.GetValue(context, this),
                k.GetValue(context, this), constraints.GetValue(context, this)
            );
            return new PointCollection2D(){points = points};
        }
    }
}