using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.Runtime;
namespace RobertHoudin.Scatter.NodeLibrary.ObjectProviders
{
    [Serializable] public class ObjectProviderPort : RhSinglePort<IObjectProvider>
    {
    }
    
    [Serializable] public class ObjectProviderPortDs : RhDataSourcePort<IObjectProvider>
    {
    }
}