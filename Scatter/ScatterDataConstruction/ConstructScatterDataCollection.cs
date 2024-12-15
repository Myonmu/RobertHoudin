using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.NodeLibrary.Loop;
using UnityEngine;
namespace RobertHoudin.Scatter.ScatterDataConstruction
{
    public class ConstructScatterDataCollection: 
        ForEachNode<Vector2CollectionPort, Vector2Port, ScatterDataPort, ScatterDataCollectionPort, Vector2, ScatterData>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            collectionOutput.value ??= new();
            collectionOutput.value.datas ??= new();
            var data = collectionOutput.value.datas;
            data.Clear();
        }

        protected override int GetInputCollectionSize(Vector2CollectionPort port)
        {
            return port.value.Count;
        }

        protected override Vector2 Extract(Vector2CollectionPort input, int i)
        {
            return input.value[i];
        }

        protected override void Put(ScatterDataCollectionPort outputPort, int i, ScatterData value)
        {
            outputPort.value.datas[i] = value;
        }
    }
}