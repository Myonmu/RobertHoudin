using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Type = System.Type;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public enum PortType
    {
        Input,
        Output
    }

    [Serializable]
    public abstract class RhPort
    {
        public virtual PortType Type { get; }
        public abstract RhPort[] GetConnectedPorts();
        public RhNode node;
        [SerializeField] private string _guid;
        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = UnityEditor.GUID.Generate().ToString();
                }

                return _guid;
            }
        }

        public virtual Type AcceptedType { get; }

        public abstract void SetValue(object value);
        public abstract object GetValue();

        public abstract void Connect(RhPort toPort);
        public abstract void Disconnect(RhPort port);
        public abstract void ForwardValue(RhPort target);
    }
    
    [Serializable]
    public abstract class RhPort<T> : RhPort
    {
        public T value;
        public override Type AcceptedType => typeof(T);
        public override object GetValue() => value;
        public override void SetValue(object value)
        {
            this.value = (T)value;
        }

        public T GetValueNoBoxing()
        {
            return value;
        }

        public void SetValueNoBoxing(T value)
        {
            this.value = value;
        }
    }
    
    public interface IRhMultiPort
    {
    }

    [Serializable]
    public abstract class RhMultiPort<T> : RhPort<T>, IRhMultiPort
    {
        [SerializeReference] public List<RhPort> connectedPorts = new();
        public override Type AcceptedType => typeof(T);
        public List<RhPort> ConnectedPorts => connectedPorts;
        
        public static implicit operator T(RhMultiPort<T> port)
        {
            return port.value;
        }

        public override void Connect(RhPort toPort)
        {
            connectedPorts.Add((RhPort<T>)toPort);
        }

        public override void Disconnect(RhPort port)
        {
            connectedPorts.Remove((RhPort<T>)port);
        }

        public override RhPort[] GetConnectedPorts()
        {
            return connectedPorts.Cast<RhPort>().ToArray();
        }

        public void ForEachConnected(Action<T> action)
        {
            foreach (var connectedPort in connectedPorts)
            {
                action((connectedPort as RhPort<T>).value);
            }
        }

        public override void ForwardValue(RhPort target)
        {
            if (target is RhSinglePort<T> singlePort)
            {
                singlePort.SetValueNoBoxing(value);
            }
            else if (target is RhMultiPort<T> multiPort)
            {
                return;
            }
        }
    }
    
    public interface IRhSinglePort
    {
    }

    [Serializable]
    public abstract class RhSinglePort<T> : RhPort<T>, IRhSinglePort
    {
        public override RhPort[] GetConnectedPorts()
        {
            return new[] { ConnectedPort };
        }

        public override void Connect(RhPort toPort)
        {
            connectedPort = (RhPort<T>)toPort;
        }

        public override void Disconnect(RhPort port)
        {
            if (connectedPort == port)
            {
                connectedPort = null;
            }
        }

        [SerializeReference] public RhPort connectedPort;
        public virtual RhPort ConnectedPort => connectedPort;

        public static implicit operator T(RhSinglePort<T> port)
        {
            return port.value;
        }

        public override void ForwardValue(RhPort target)
        {
            if (target is RhSinglePort<T> singlePort)
            {
                singlePort.SetValueNoBoxing(value);
            }
            else if (target is RhMultiPort<T> multiPort)
            {
                return;
            }
        }
    }
}