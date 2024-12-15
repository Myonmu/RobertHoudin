using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class Vector2ToVector3FuncPort : RhMultiPort<Func<Vector2, Vector3>> {}
    [Serializable] public class Vector2ToQuaternionFuncPort : RhMultiPort<Func<Vector2, Quaternion>> {}
}