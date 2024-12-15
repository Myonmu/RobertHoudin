using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
namespace RobertHoudin.Framework.Core.Ports
{
    /// <summary>
    /// Generalized int/float port, simplifies node management.
    /// </summary>
    /// <remarks>
    /// if your number is large enough that can cause float overflow, you must use a dedicated port.
    /// </remarks>
    [Serializable] public class NumberPort: RhSinglePort<Number> { }
    
    [Serializable] public class MultiNumberPort: RhMultiPort<Number> { }
}