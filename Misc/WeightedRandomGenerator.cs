using System;
using System.Collections.Generic;
namespace RobertHoudin.Misc
{
    public class WeightedRandomGenerator<T>
    {
        private Dictionary<T, float> _weightMap = new();
        private float _accumulatedWeight;

        public float this[T v]
        {
            get => _weightMap[v];
            set => SetWeight(v,value);
        } 
        
        /// <summary>
        /// Add a new object and its associated weight to the generator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="weight"></param>
        public void Add(T value, float weight)
        {
            _weightMap.Add(value, weight);
            _accumulatedWeight += weight;
        }

        public void Remove(T value)
        {
            _accumulatedWeight -= _weightMap[value];
            _weightMap.Remove(value);
        }

        public void SetWeight(T value, float weight)
        {
            _accumulatedWeight += (weight - (_weightMap.ContainsKey(value) ? _weightMap[value] : 0));
            _weightMap[value] = weight;
        }
        
        public T GetRandom()
        {
            if(_accumulatedWeight == 0) 
                throw new InvalidOperationException("Accumulated weight is zero, the generator will unable to select a value.");
            var random = new Random();
            var randomValue = (float)random.NextDouble() * _accumulatedWeight;
            var w = 0f;
            foreach (var t in _weightMap)
            {
                w += t.Value;
                if (randomValue < w)
                {
                    return t.Key;
                }
            }
            throw new Exception("Unable to select a value.");
        }
    }
}