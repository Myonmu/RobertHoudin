using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.NodeLibrary.Loop;

namespace RobertHoudin.NodeLibrary
{
    [Serializable]
    public class ForeachInt : ForEachNode<IntCollectionPort, IntPort, IntPort, IntCollectionPort, int, int>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            base.OnBeginEvaluate(context);
            collectionOutput.value.list.Clear();
        }

        protected override int GetInputCollectionSize(IntCollectionPort port)
        {
            if (port.value == null) port.value = new();
            return port.value.list.Count;
        }

        protected override int Extract(IntCollectionPort input, int i)
        {
            return input.value.list[i];
        }

        protected override void Put(IntCollectionPort outputPort, int i, int value)
        {
            outputPort.value.list.Add(value);
        }
    }
}