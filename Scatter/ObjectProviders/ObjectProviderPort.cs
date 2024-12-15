using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Scatter.ObjectProviders
{
    [Serializable] public class ObjectProviderPort : RhSinglePort<IObjectProvider>
    {
    }
    
    [Serializable] public class ObjectProviderPortDs : RhDataSourcePort<IObjectProvider>
    {
    }
}