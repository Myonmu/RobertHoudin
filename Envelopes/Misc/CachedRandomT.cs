using System;
using System.Collections.Generic;
namespace RobertHoudin.Envelopes.Misc
{
    public class CachedRandomT<T>
    {
        public virtual int Seed { get; set; } = 0;
        private List<T> _cache = new();
        private Random _random;
        private Func<Random, int, T> _gen;

        public int Count => _cache.Count;
        public CachedRandomT(Func<Random, int, T> gen)
        {
            _gen = gen;
            _random = new Random();
        }
        public virtual void Regenerate()
        {
            _random = new Random(Seed);
            for (var i = 0; i < _cache.Count; i++)
            {
                _cache[i] = _gen(_random, _cache.Count);
            }
        }
        
        public void Resize(int length)
        {
            while (_cache.Count > length)
            {
                _cache.RemoveAt(_cache.Count - 1);
            }

            while (_cache.Count < length)
            {
                _cache.Add(_gen(_random, length));
            }
        }
        
        public T Get(int i)
        {
            return _cache[i];
        }
    }
}