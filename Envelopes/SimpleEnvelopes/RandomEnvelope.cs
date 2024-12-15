using RobertHoudin.Envelopes.Base;
using Sirenix.OdinInspector;
using UnityEngine;
namespace RobertHoudin.Envelopes.SimpleEnvelopes
{
    public class RandomRangeEnvelope: EnvelopeBase
    {
        public bool forceConstant;
        [HideIf("forceConstant")]
        public Vector2 randomRange;

        [ShowIf("forceConstant")]
        public float constValue;
        public override float Evaluate(float i)
        {
            if (forceConstant) return constValue;
            return UnityEngine.Random.Range(randomRange.x, randomRange.y);
            //return (float)new Random().NextDouble() * (randomRange.y - randomRange.x) + randomRange.x;
        }
    }
}