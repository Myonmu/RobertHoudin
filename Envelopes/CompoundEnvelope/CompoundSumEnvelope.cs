using System;

namespace TATools.RobertHoudin
{
    [Serializable]
    public class CompoundSumEnvelope: CompoundEnvelopeBase
    {
        public override float Evaluate(float i)
        {
            var result = 1f;
            foreach (var envelop in envelopes)
            {
                result += envelop.IsActive ? envelop.Evaluate(i) : 0;
            }

            return result;
        }

       
    }
}