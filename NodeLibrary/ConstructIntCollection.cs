using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary
{
    [Serializable]
    public class ConstructIntCollection: RhNode
    {
        [RhInputPort] public MultiIntPort ints = new();
        [RhOutputPort] public IntCollectionPort output = new();
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.value.list.Clear();
            ints.ForEachConnected((i) =>
            {
                output.value.list.Add(i);
            });
            return true;
        }
    }
}