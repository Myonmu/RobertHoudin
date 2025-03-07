using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.Runtime;
namespace RobertHoudin.Scatter.NodeLibrary
{
    [Serializable] public class ScatterDataPort : RhSinglePort<ScatterData> {}
    [Serializable] public class ScatterDataCollectionPort : RhListCollectionPort<ScatterData> {}
}