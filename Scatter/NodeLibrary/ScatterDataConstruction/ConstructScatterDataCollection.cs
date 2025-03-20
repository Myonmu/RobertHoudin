using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using RobertHoudin.NodeLibrary.Loop;
using RobertHoudin.Scatter.Runtime;
namespace RobertHoudin.Scatter.NodeLibrary.ScatterDataConstruction
{
    public class ConstructScatterDataCollection: 
        ForEachNode<NumberBufferPort, VectorPort, ScatterDataPort, ScatterDataCollectionPort, Vector, ScatterData>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            base.OnBeginEvaluate(context);
            collectionOutput.value ??= new();
            var data = collectionOutput.value;
            data.Clear();
        }

        protected override int GetInputCollectionSize(NumberBufferPort port)
        {
            return port.GetValueNoBoxing().Count;
        }

        protected override Vector Extract(NumberBufferPort input, int i)
        {
            return input.GetValueNoBoxing()[i];
        }

        protected override void Put(ScatterDataCollectionPort outputPort, int i, ScatterData value)
        {
            outputPort.GetValueNoBoxing().Add(value);
        }
    }
}