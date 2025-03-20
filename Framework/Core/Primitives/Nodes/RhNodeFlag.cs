using System;
namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    [Flags]
    public enum RhNodeFlag
    {
        None,
        PreventLoopResetPropagate
    }
}