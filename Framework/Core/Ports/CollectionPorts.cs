using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEngine;
namespace RobertHoudin.Framework.Core.Ports
{
    [Serializable]
    public abstract class ListCollection<T>
    {
        public List<T> collection = new();
    }

    [Serializable] public abstract class RhListCollectionPort<T> : RhSinglePortObjectType<List<T>>{}

    [Serializable]public class IntCollectionPort : RhListCollectionPort<int>{}
    [Serializable]public class NumberCollectionPort : RhListCollectionPort<Number>{}
    [Serializable]public class FloatCollectionPort : RhListCollectionPort<float>{}
    [Serializable]public class Vector2CollectionPort : RhListCollectionPort<Vector2>{}
    [Serializable]public class Vector3CollectionPort : RhListCollectionPort<Vector3>{}
}