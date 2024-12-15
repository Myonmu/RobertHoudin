using RobertHoudin.Envelopes.Base;
using Sirenix.OdinInspector;
using UnityEngine;
namespace RobertHoudin.Envelopes.SimpleEnvelopes
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