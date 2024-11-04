using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.DataProcessors
{
    public class BaseDataProcessor : ScriptableObject
    {
        public virtual object Process(object o) { return o; }
    }
}