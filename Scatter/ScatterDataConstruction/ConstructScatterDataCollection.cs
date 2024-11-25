using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.NodeLibrary.Loop;
using TATools.RobertHoudin.Framework.Core.Ports;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    public class ConstructScatterDataCollection: 
        ForEachNode<PointCollection2DPort, Vector2Port, ScatterDataPort, ScatterDataCollectionPort, Vector2, ScatterData>
    {
        protected override void OnBeginEvaluate(RhExecutionContext context)
        {
            collectionOutput.value ??= new();
            collectionOutput.value.datas ??= new();
            var data = collectionOutput.value.datas;
            data.Clear();
        }

        protected override int GetInputCollectionSize(PointCollection2DPort port)
        {
            return port.value.points.Count;
        }

        protected override Vector2 Extract(PointCollection2DPort input, int i)
        {
            return input.value.points[i];
        }

        protected override void Put(ScatterDataCollectionPort outputPort, int i, ScatterData value)
        {
            outputPort.value.datas[i] = value;
        }
    }
}