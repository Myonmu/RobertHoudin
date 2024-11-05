using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    
    public struct ScatterData
    {
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;
    }

    public class ScatterDataCollection
    {
        public List<ScatterData> datas = new();
    }
    
    [Serializable] public class ScatterDataCollectionPort : RhSinglePort<ScatterDataCollection> {}
}