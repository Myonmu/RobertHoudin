using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.MochiVariable
{
    public abstract class BindingSource
    {
        public virtual Object BaseObj { get; set; }
        public virtual Object UnityObj { get; set; }
        public string selectedProperty;
        public string selectedSub;
        
    }
}