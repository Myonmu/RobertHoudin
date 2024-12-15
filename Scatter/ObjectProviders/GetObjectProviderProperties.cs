using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.Scatter.ObjectProviders
{
    public class GetObjectProviderProperties: RhNode
    {
        [RhInputPort] public ObjectProviderPortDs objectProvider;
        [RhOutputPort] public NumberPort indexMin;
        [RhOutputPort] public NumberPort indexMax;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            indexMin.SetValueNoBoxing(objectProvider.GetValueNoBoxing().MinIndex);
            indexMax.SetValueNoBoxing(objectProvider.GetValueNoBoxing().MaxIndex);
            return true;
        }
    }
}