using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace TATools.RobertHoudin.Framework.Core.Ports
{
    [Serializable] public class Texture2DPort: RhSinglePort<Texture2D> { }
    [Serializable] public class FloatPort: RhSinglePort<float> { }
    [Serializable] public class ColorPort: RhSinglePort<Color> { }
    [Serializable] public class IntPort: RhSinglePort<int> { }
    [Serializable] public class Vector2Port: RhSinglePort<Vector2> { }
    [Serializable] public class Vector3Port: RhSinglePort<Vector3> { }
    [Serializable] public class Vector4Port: RhSinglePort<Vector4> { }
    [Serializable] public class QuaternionPort: RhSinglePort<Quaternion> { }
    [Serializable] public class Vector3IntPort: RhSinglePort<Vector3Int> { }
    [Serializable] public class Vector2IntPort: RhSinglePort<Vector2Int> { }
    
    [Serializable] public class BoundsPort: RhSinglePort<Bounds> { }

    [Serializable] public class MultiIntPort : RhMultiPort<int> { }
    
    [Serializable] public class GameObjectPort : RhSinglePort<GameObject> {}
    [Serializable] public class TransformPort : RhSinglePort<Transform> {}
}