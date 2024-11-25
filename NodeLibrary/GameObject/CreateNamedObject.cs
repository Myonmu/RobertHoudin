using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using TATools.RobertHoudin.Framework.Core.Ports;

namespace RobertHoudin.NodeLibrary.GameObject
{
    public class CreateNamedObject: RhNode
    {
        [RhOutputPort] public GameObjectPort output;
        
        public DataSource<string> objectName;
        public DataSource<bool> alwaysCreateNew;
        
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var o = objectName.GetValue(context, this);
            if (!alwaysCreateNew.GetValue(context, this))
            {
                var existing = UnityEngine.GameObject.Find(o);
                if (existing)
                {
                    output.SetValueNoBoxing(existing);
                    return true;
                }
            }
            output.SetValueNoBoxing(new UnityEngine.GameObject(o));
            return true;
        }
    }
}