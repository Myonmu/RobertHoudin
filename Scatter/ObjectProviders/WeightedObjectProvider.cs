using System;
using System.Collections.Generic;
using RobertHoudin.Misc;
using UnityEngine;
namespace RobertHoudin.Scatter.ObjectProviders
{
    [CreateAssetMenu(fileName = "SimpleObjectProvider", menuName = "RobertHoudin/Scatter/Weighted Object Provider")]
    public class WeightedObjectProvider: ScriptableObject, IObjectProvider
    {
        [Serializable]
        public class WeightedObject
        {
            /// <summary>
            /// When disabled, weight value will be interpreted as 0.
            /// see <see cref="WeightedObjectProvider.UpdateWeights"/>
            /// </summary>
            public bool enabled = true;
            public GameObject prefab;
            public float weight = 1;
        }

        public List<WeightedObject> prefabs = new();
        /// <summary>
        /// We store index rather than GameObject here
        /// </summary>
        private WeightedRandomGenerator<int> _random;

        private void UpdateWeights()
        {
            _random??= new WeightedRandomGenerator<int>();
            var id = 0;
            foreach (var prefab in prefabs)
            {
                _random[id] = prefab.enabled ? prefab.weight : 0;
                id++;
            }
        }
        public GameObject GetObjectByIndex(int objectId)
        {
            return prefabs[objectId].prefab;
        }
        
        public int GetRandomObjectIndex()
        {
            UpdateWeights();
            return _random.GetRandom();
        }
        /// <summary>
        /// Pretty much useless
        /// </summary>
        public int MinIndex
        {
            get;
        }
        public int MaxIndex
        {
            get;
        }
    }
}