using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Misc
{
    public class PrintNode: RhNode
    {
        [RhInputPort] public StringPort str;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            Debug.Log(str.GetValueNoBoxing());
            return true;
        }
    }
}