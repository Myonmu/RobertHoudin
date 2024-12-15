﻿using System.Collections.Generic;
using UnityEngine;
namespace RobertHoudin.Scatter.ObjectProviders
{
    [CreateAssetMenu(fileName = "SimpleObjectProvider", menuName = "RobertHoudin/Scatter/SimpleObjectProvider")]
    public class SimpleObjectProvider: ScriptableObject,IObjectProvider
    {
        public List<GameObject> prefabs = new();
        public GameObject GetObjectByIndex(int objectId)
        {
            if(objectId < 0 || objectId >= prefabs.Count) return null;
            return prefabs[objectId];
        }
        public int MinIndex => 0;
        public int MaxIndex => prefabs.Count;


    }
}