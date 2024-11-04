using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.DataProcessors
{
    public class BaseMultiDataProcessor : ScriptableObject
    {
        public virtual object Process(object a, object b) { return a; }
    }
}