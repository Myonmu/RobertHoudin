﻿using UnityEngine;
namespace RobertHoudin.Scatter.Runtime
{
    /// <summary>
    /// Contains enough data to place an object in the world
    /// </summary>
    public struct ScatterData
    {
        public bool isDiscarded;
        public Vector3 pos;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 normal;
        public int objectId;
    }
}