using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using RobertHoudin.NodeLibrary.Loop;
namespace RobertHoudin.NodeLibrary.Numeric
{
    [Serializable]
    public class ForeachNumber : ForEachNode<NumberCollectionPort, 
        NumberPort, NumberPort, NumberCollectionPort, Number, Number>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            base.OnBeginEvaluate(context);
            collectionOutput.value.Clear();
        }

        protected override int GetInputCollectionSize(NumberCollectionPort port)
        {
            if (port.value == null) port.value = new();
            return port.value.Count;
        }

        protected override Number Extract(NumberCollectionPort input, int i)
        {
            return input.value[i];
        }

        protected override void Put(NumberCollectionPort outputPort, int i, Number value)
        {
            outputPort.value.Add(value);
        }
    }
}