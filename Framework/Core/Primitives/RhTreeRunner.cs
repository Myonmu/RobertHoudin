using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Events;

namespace RobertHoudin.Framework.Core.Primitives
{
    public class RhTreeRunner : IListener
    {
        public RhTree tree;
        public RhPropertyBlock agent;
        public void Init()
        {
            tree = tree.Clone();
            ResetTree();
        }
        public void ResetTree()
        {
            tree.ResetTree();
        }
        public void Update()
        {
            tree.EvaluateTree(agent);
        }
    }
}