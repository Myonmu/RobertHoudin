using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
namespace RobertHoudin.NodeLibrary.Vector
{
    public class VectorToUnityVector: RhNode
    {
        [RhNodeData] public string subscript = "xyzw";
        [RhInputPort] public VectorPortDs vector;
        [RhOutputPort] public NumberPort vector1;
        [RhOutputPort] public Vector2Port vector2;
        [RhOutputPort] public Vector3Port vector3;
        [RhOutputPort] public Vector4Port vector4;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var s = new VectorSubscript(subscript, vector.GetValueNoBoxing().ComponentCount);
            if (vector1.IsActive)
            {
                vector1.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript1(s));
            }
            if (vector2.IsActive)
            {
                vector2.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript2(s));
            }
            if (vector3.IsActive)
            {
                vector3.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript3(s));
            }
            if (vector4.IsActive)
            {
                vector4.SetValueNoBoxing(vector.GetValueNoBoxing().Subscript4(s));
            }
            return true;
        }
    }
}