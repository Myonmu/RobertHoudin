using System;
namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    
    [AttributeUsage(AttributeTargets.Field)]
    public class RhOutputPortAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RhInputPortAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class RhLoopItemPortAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class RhLoopResultPortAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RhDataSourcePortConnectAttribute : Attribute
    {
        public int portNumber;

        public RhDataSourcePortConnectAttribute(int portNumber)
        {
            this.portNumber = portNumber;
        }
    }
}