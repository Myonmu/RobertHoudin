using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class IntPortDs : RhDataSourcePort<int> {}
    [Serializable] public class Texture2DPortDs: RhDataSourcePort<Texture2D> { }
    [Serializable] public class FloatPortDs: RhDataSourcePort<float> { }
    [Serializable] public class ColorPortDs: RhDataSourcePort<Color> { }
    
    [Serializable] public class NumberPortDs: RhDataSourcePort<Number> { }
    
    [Serializable] public class Vector2PortDs: RhDataSourcePort<Vector2> { }
    [Serializable] public class Vector3PortDs: RhDataSourcePort<Vector3> { }
    [Serializable] public class Vector4PortDs: RhDataSourcePort<Vector4> { }
    [Serializable] public class QuaternionPortDs: RhDataSourcePort<Quaternion> { }
    [Serializable] public class Vector3IntPortDs: RhDataSourcePort<Vector3Int> { }
    [Serializable] public class Vector2IntPortDs: RhDataSourcePort<Vector2Int> { }
    [Serializable] public class BoundsPortDs: RhDataSourcePort<Bounds> { }
}