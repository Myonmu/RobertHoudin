using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    
    public struct ScatterData
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 normal;
    }

    public class ScatterDataCollection
    {
        public List<ScatterData> datas = new();
    }
    
    [Serializable] public class ScatterDataPort : RhSinglePort<ScatterData> {}
    [Serializable] public class ScatterDataCollectionPort : RhSinglePort<ScatterDataCollection> {}
}