using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.NodeLibrary.GameObject
{
    public class GetTransform: RhNode
    {
        [RhInputPort] public GameObjectPort input;
        [RhOutputPort] public TransformPort output;
        public DataSource<UnityEngine.GameObject> gameObject;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var go = gameObject.GetValue(context, this);
            if (go == null) return false;
            output.SetValueNoBoxing(go.transform);
            return true;
        }
    }
}