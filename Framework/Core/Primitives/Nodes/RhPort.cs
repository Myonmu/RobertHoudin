using System;
using System.Collections.Generic;
using System.Linq;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public enum PortType
    {
        Input, Output
    }
    
    [Serializable]
    public abstract class RhPort
    {
        public virtual PortType Type { get; }
        public abstract RhPort[] GetConnectedPorts();
        public RhNode node;
        public virtual Type AcceptedType { get; }
    }

    public abstract class RhPort<T> : RhPort
    {
        public override Type AcceptedType => typeof(T);
    }

    public class RhMultiPort : RhPort
    {
        public virtual List<RhPort> ParentPorts { get; }
        public override RhPort[] GetConnectedPorts()
        {
            return ParentPorts.ToArray();
        }
    }

    public abstract class RhMultiPort<T>: RhMultiPort
    {
        public List<RhPort<T>> parentPorts;
        public override Type AcceptedType => typeof(T);
        public override List<RhPort> ParentPorts => parentPorts.Cast<RhPort>().ToList();
    }

    public class RhSinglePort : RhPort
    {
        public override RhPort[] GetConnectedPorts()
        {
            return new[] { ParentPort };
        }

        public virtual RhPort ParentPort { get; }
    }
    public class RhSinglePort<T>: RhSinglePort
    {
        public override Type AcceptedType => typeof(T);
        public RhPort<T> parentPort;
        public override RhPort ParentPort => parentPort;

        public T value;
    }
}