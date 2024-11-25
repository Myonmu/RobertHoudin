using System;

namespace TATools.RobertHoudin
{
    [Serializable]
    public abstract class EnvelopeBase: IEnvelope
    {
        [OnValueChanged("@IsDirty = true")][ToggleLeft]
        public bool enabled = true;
        public bool IsActive => enabled;
        public abstract float Evaluate(float i);
        public virtual bool IsDirty { get; set; }
    }
}