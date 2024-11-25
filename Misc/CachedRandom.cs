using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

namespace TATools.RobertHoudin
{
    /// <summary>
    /// Cached list of random numbers. Useful when you need to generate consistent
    /// results based on randomizer params (seed, range) in a multithreaded scenario.
    /// (System.Random is not thread-safe, and UnityEngine.Random cannot be multithreaded).
    /// Provides "Envelop" option that can be used to control the random pattern.
    /// </summary>
    [Serializable]
    public class CachedRandom
    {
        [OnValueChanged("Regenerate")]
        public int seed;
        private List<float> _cache = new();
        private Random _random = new();
        [OnValueChanged("Regenerate")] public Vector2 range = new Vector2(-1, 1);
        [FormerlySerializedAs("envelop")] [OnValueChanged("Regenerate")] [SerializeReference] public IEnvelope envelope;

        public void RegenerateIfDirty()
        {
            if (envelope is { IsDirty: true })
            {
                Regenerate();
                envelope.IsDirty = false;
            }
        }
        
        public void Regenerate()
        {
            _random = new Random(seed);
            for (var i = 0; i < _cache.Count; i++)
            {
                _cache[i] = GenerateRandom(i/(float)_cache.Count);
            }
        }

        private float EvaluateEnvelop(IEnvelope envelop, float pos, float nullVal)
        {
            return envelop is not { IsActive: true } ? nullVal : envelop.Evaluate(pos);
        }

        private float GenerateRandom(float pos)
        {
            return ((float)_random.NextDouble() * (range.y - range.x) + range.x) *
                   EvaluateEnvelop(envelope, pos, 1);
        }

        public void Resize(int length)
        {
            while (_cache.Count > length)
            {
                _cache.RemoveAt(_cache.Count - 1);
            }

            while (_cache.Count < length)
            {
                _cache.Add(GenerateRandom(_cache.Count / (float)length));
            }
        }

        public float Get(int i)
        {
            return _cache[i];
        }
    }
}