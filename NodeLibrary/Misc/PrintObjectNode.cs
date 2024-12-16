using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Misc
{
    public class PrintObjectNode: RhNode
    {
        [RhInputPort] public BoxedObjectPort content;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            Debug.Log(content.GetValueNoBoxing());
            return true;
        }
    }
}