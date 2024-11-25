using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TATools.RobertHoudin
{
    [Serializable]
    public class AdvancedSinusoidalEnvelope: EnvelopeBase
    {
        [SerializeReference][OnValueChanged("@IsDirty = true")] public IEnvelope frequency = new ConstantEnvelope();
        [SerializeReference][OnValueChanged("@IsDirty = true")] public IEnvelope phase = new ConstantEnvelope();
        [SerializeReference][OnValueChanged("@IsDirty = true")] public IEnvelope amplitude = new ConstantEnvelope();
        
        [OnValueChanged("@IsDirty = true")] public bool highCut = false;
        [SerializeReference][OnValueChanged("@IsDirty = true")][ShowIf("highCut")] public IEnvelope highCutStart = new ConstantEnvelope();
        [SerializeReference][OnValueChanged("@IsDirty = true")][ShowIf("highCut")] public IEnvelope highCutValue = new ConstantEnvelope();
        
        [OnValueChanged("@IsDirty = true")] public bool lowCut = false;
        [SerializeReference][OnValueChanged("@IsDirty = true")][ShowIf("lowCut")] public IEnvelope lowCutStart = new ConstantEnvelope();
        [SerializeReference][OnValueChanged("@IsDirty = true")][ShowIf("lowCut")] public IEnvelope lowCutValue = new ConstantEnvelope();
        public override float Evaluate(float i)
        {
            var val = Mathf.Sin(i * frequency.Evaluate(i) + phase.Evaluate(i)) * amplitude.Evaluate(i);
            if (highCut && val > highCutStart.Evaluate(i)) val = highCutValue.Evaluate(i);
            if (lowCut && val < lowCutStart.Evaluate(i)) val = lowCutValue.Evaluate(i);
            return val;
        }

        public override bool IsDirty
        {
            get
            {
                return frequency.IsDirty || phase.IsDirty || amplitude.IsDirty || lowCutStart.IsDirty ||
                       lowCutValue.IsDirty ||
                       highCutStart.IsDirty || highCutValue.IsDirty;
            }
            set
            {
                frequency.IsDirty = value;
                phase.IsDirty = value;
                amplitude.IsDirty = value;
                lowCutStart.IsDirty = value;
                lowCutValue.IsDirty = value;
                highCutStart.IsDirty = value;
                highCutValue.IsDirty = value;
            }
        }
    }
}