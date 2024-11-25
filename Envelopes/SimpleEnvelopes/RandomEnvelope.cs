using Sirenix.OdinInspector;
using UnityEngine;

namespace TATools.RobertHoudin
{
    public class RandomRangeEnvelope: EnvelopeBase
    {
        [Label("输出定值")]
        public bool forceConstant;
        [HideIf("forceConstant")][Label("随机范围")]
        public Vector2 randomRange;

        [Label("定值")][ShowIf("forceConstant")]
        public float constValue;
        public override float Evaluate(float i)
        {
            if (forceConstant) return constValue;
            return UnityEngine.Random.Range(randomRange.x, randomRange.y);
            //return (float)new Random().NextDouble() * (randomRange.y - randomRange.x) + randomRange.x;
        }
    }
}