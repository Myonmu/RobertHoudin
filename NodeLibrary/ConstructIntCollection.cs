using System;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

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