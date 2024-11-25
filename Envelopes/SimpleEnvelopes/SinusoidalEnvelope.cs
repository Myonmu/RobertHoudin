using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityExtensions;

namespace TATools.RobertHoudin
{
    [Serializable]
    public class SinusoidalEnvelope : EnvelopeBase
    {
        [OnValueChanged("@IsDirty = true")] public float frequency;
        [OnValueChanged("@IsDirty = true")] public float phase;
        [OnValueChanged("@IsDirty = true")] public float amplitude;
        
        [OnValueChanged("@IsDirty = true")] public bool highCut = false;
        [OnValueChanged("@IsDirty = true")][ShowIf("highCut")]
        public float highCutStart;
        [OnValueChanged("@IsDirty = true")][ShowIf("highCut")]
        public float highCutValue;
        
        [OnValueChanged("@IsDirty = true")] public bool lowCut = false;
        [OnValueChanged("@IsDirty = true")][ShowIf("lowCut")]
        public float lowCutStart;
        [OnValueChanged("@IsDirty = true")][ShowIf("lowCut")]
        public float lowCutValue;

        public override float Evaluate(float i)
        {
            var val = Mathf.Sin(i * frequency + phase) * amplitude;
            if (highCut && val > highCutStart) val = highCutValue;
            if (lowCut && val < lowCutStart) val = lowCutValue;
            return val;
        }
    }
}