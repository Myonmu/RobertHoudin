using System;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    
    [AttributeUsage(AttributeTargets.Field)]
    public class RhOutputPortAttribute:Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RhInputPortAttribute : Attribute
    {
        
    }
}