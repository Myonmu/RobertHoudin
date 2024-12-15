
using RobertHoudin.Envelopes.Base;
namespace RobertHoudin.Envelopes.SimpleEnvelopes
{
    public class ConstantEnvelope: EnvelopeBase
    {
        public float constantValue;
        public override float Evaluate(float i)
        {
            return constantValue;
        }
    }
}