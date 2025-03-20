using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Vector
{
    public class ExtractVectorFromBuffer: RhNode
    {
        [RhInputPort] public NumberBufferPort buffer;
        [RhInputPort] public NumberPortDs index;
        [RhOutputPort] public VectorPort vector;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            if (index.GetValueNoBoxing() >= buffer.GetValueNoBoxing().Count)
            {
                Debug.LogError($"Index out of range: {index.GetValueNoBoxing()} on buffer with size {buffer.GetValueNoBoxing().Count}");
                return false;
            }
            vector.SetValueNoBoxing(buffer.GetValueNoBoxing().GetVectorAt(index.GetValueNoBoxing()));
            return true;
        }
    }
}