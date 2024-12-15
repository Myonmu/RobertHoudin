using RobertHoudin.Framework.Core.Primitives.DataContainers;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    /// <summary>
    /// External context when evaluating an RhTree.
    /// "External" means the data is passed in rather than
    /// being part of the tree iteself.
    /// </summary>
    public class RhExecutionContext
    {
        public RhPropertyBlock propertyBlock;
    }
}