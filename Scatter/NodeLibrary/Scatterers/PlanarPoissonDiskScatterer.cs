﻿using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using RobertHoudin.Scatter.Runtime;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.Scatterers
{
    [Serializable]
    public class PoissonDiskConstraintPort : RhSinglePort<FastPoissonDiskGenerator.PoissonDiskConstraint>
    {
    }

    public class PlanarPoissonDiskScatterer : PlanarScatterer, IPlanarScatterer
    {
        [RhInputPort] public IntPortDs maxActivePointsPort;
        [RhInputPort] public BoundsPortDs boundsPort;
        [RhInputPort] public IntPortDs kPort;
        [RhInputPort] public Vector2PortDs distancePort;
        [RhInputPort] public PoissonDiskConstraintPort constraintsPort;
        [RhInputPort] public Vector2PredicatesPort predicatesPort;
        
        public override NumberBuffer Scatter(RhExecutionContext context)
        {
            var gen = new FastPoissonDiskGenerator()
            {
                maxActivePointCount = maxActivePointsPort.GetValueNoBoxing()
            };
            var b = boundsPort.GetValueNoBoxing();
            
            predicatesPort.ForEachConnected((predicate) =>
            {
                gen.AddPredicate(predicate);
            });
            
            var points = gen.Sample(new Vector2(b.min.x, b.min.z),
                new Vector2(b.max.x, b.max.z), distancePort.GetValueNoBoxing(),
                kPort.GetValueNoBoxing(), constraintsPort.GetValueNoBoxing()
            );
            return points;
        }

        public void Scatter(ref List<Vector2> resultsCollection)
        {
            
        }

        public void UpdateConfig(IScatterConfigChunk configChunk)
        {
            
        }
    }
}