using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class Vector2PredicatesPort : RhMultiPort<Func<Vector2, bool>> {}
}