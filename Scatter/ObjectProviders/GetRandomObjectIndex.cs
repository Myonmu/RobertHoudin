﻿using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.Scatter.ObjectProviders
{
    public class GetRandomObjectIndex: RhNode
    {
        [RhInputPort] public ObjectProviderPortDs objectProvider;
        [RhOutputPort] public IntPort randomIndex;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            randomIndex.SetValueNoBoxing(objectProvider.GetValueNoBoxing().GetRandomObjectIndex());
            return true;
        }
    }
}