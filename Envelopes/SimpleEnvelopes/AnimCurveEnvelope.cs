using Sirenix.OdinInspector;
using UnityEngine;

namespace TATools.RobertHoudin
{
    public class AnimCurveEnvelope: EnvelopeBase
    {
        [OnValueChanged("@IsDirty = true")]
        public AnimationCurve curve;

        public override float Evaluate(float i)
        {
            return curve.Evaluate(i);
        }
    }
}