using System;
using UnityEngine;
namespace RobertHoudin.Envelopes.Misc
{
    [Serializable]
    public class CachedRandomLinearTransform
    {
        public CachedRandom k = new();
        public CachedRandom c = new();
        public Vector2 ValueRange => new Vector2(k.range.x + c.range.x, k.range.y + c.range.y);
        public float Get(int i, float originalValue)
        {
            return k.Get(i)*originalValue + c.Get(i);
        }

        public void RegenerateIfDirty()
        {
            k.RegenerateIfDirty();
            c.RegenerateIfDirty();
        }

        public void Resize(int cnt)
        {
            k.Resize(cnt);
            c.Resize(cnt);
        }
    }
}