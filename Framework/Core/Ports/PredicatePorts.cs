using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class Point2DPredicatesPort : RhMultiPort<Func<Vector2, bool>> {}
}