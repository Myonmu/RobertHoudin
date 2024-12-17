using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using Sirenix.OdinInspector;
using UnityEngine;
namespace RobertHoudin.NodeLibrary.Misc
{
    public class PrintNode: RhNode
    {
        [RhNodeData] public string someString;
        [RhNodeData] public Bounds bounds;
        [RhInputPort] public StringPort str;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            Debug.Log(str.GetValueNoBoxing());
            return true;
        }
    }
}