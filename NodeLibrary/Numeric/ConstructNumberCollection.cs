using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Numeric
{
    [Serializable]
    public class ConstructNumberCollection: RhNode
    {
        [RhInputPort] public MultiNumberPort ints = new();
        [RhOutputPort] public NumberCollectionPort output = new();
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.value.Clear();
            ints.ForEachConnected((i) =>
            {
                output.value.Add(i);
            });
            return true;
        }
    }
}