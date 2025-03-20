using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class VectorPort : RhSinglePort<Vector>{}
    [Serializable] public class VectorPortDs : RhDataSourcePort<Vector>{}
}