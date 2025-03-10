﻿using System.Collections.Generic;
using RobertHoudin.Scatter.Runtime;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.ObjectProviders
{
    [CreateAssetMenu(fileName = "SimpleObjectProvider", menuName = "RobertHoudin/Scatter/Simple Object Provider")]
    public class SimpleObjectProvider: ScriptableObject,IObjectProvider
    {
        public List<GameObject> prefabs = new();
        public GameObject GetObjectByIndex(int objectId)
        {
            if(objectId < 0 || objectId >= prefabs.Count) return null;
            return prefabs[objectId];
        }
        public int GetRandomObjectIndex(int i = 0)
        {
            return Random.Range(0, prefabs.Count);
        }
        public int MinIndex => 0;
        public int MaxIndex => prefabs.Count;


    }
}