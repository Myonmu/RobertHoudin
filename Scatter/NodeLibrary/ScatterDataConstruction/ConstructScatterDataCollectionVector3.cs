using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.NodeLibrary.Loop;
using RobertHoudin.Scatter.Runtime;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.ScatterDataConstruction
{
    public class ConstructScatterDataCollectionVector3 : 
        ForEachNode<Vector3CollectionPort, Vector3Port, ScatterDataPort, 
            ScatterDataCollectionPort, Vector3, ScatterData>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            base.OnBeginEvaluate(context);
            collectionOutput.value ??= new();
            var data = collectionOutput.value;
            data.Clear();
        }

        protected override int GetInputCollectionSize(Vector3CollectionPort port)
        {
            return port.value.Count;
        }

        protected override Vector3 Extract(Vector3CollectionPort input, int i)
        {
            return input.value[i];
        }

        protected override void Put(ScatterDataCollectionPort outputPort, int i, ScatterData value)
        {
            outputPort.value.Add(value);
        }
    }
}