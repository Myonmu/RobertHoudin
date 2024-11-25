
namespace TATools.RobertHoudin
{
    public class ConstantEnvelope: EnvelopeBase
    {
        [Label("固定值")]
        public float constantValue;
        public override float Evaluate(float i)
        {
            return constantValue;
        }
    }
}