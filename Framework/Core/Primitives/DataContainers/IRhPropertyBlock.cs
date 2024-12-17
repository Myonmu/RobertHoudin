using System;
using Object = UnityEngine.Object;

namespace RobertHoudin.Framework.Core.Primitives.DataContainers
{
    public interface IRhPropertyBlock
    {
        /// <summary>
        /// Called just before passing the property block to the RhTree
        /// </summary>
        public void OnBeginEvaluate();
    }
}