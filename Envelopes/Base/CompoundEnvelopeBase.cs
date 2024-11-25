using System;
using System.Collections.Generic;
using UnityEngine;

namespace TATools.RobertHoudin
{
    [Serializable]
    public abstract class CompoundEnvelopeBase: IEnvelope
    {
        public bool enabled = true;
        [SerializeReference] public List<IEnvelope> envelopes = new();
        public bool IsActive => enabled;
        public abstract float Evaluate(float i);
        public bool IsDirty
        {
            get
            {
                foreach (var envelop in envelopes)
                {
                    if (envelop.IsDirty) return true;
                }

                return false;
            }
            set
            {
                foreach (var envelop in envelopes)
                {
                    envelop.IsDirty = value;
                }
            }
        }
    }
}