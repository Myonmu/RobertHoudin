using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Scatter.ScatterDataConstruction
{
    /// <summary>
    /// Contains enough data to place an object in the world
    /// </summary>
    public struct ScatterData
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 normal;
        public int objectId;
    }
    
    [Serializable] public class ScatterDataPort : RhSinglePort<ScatterData> {}
    [Serializable] public class ScatterDataCollectionPort : RhListCollectionPort<ScatterData> {}
}