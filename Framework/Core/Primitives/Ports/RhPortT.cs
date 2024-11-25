using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    [Serializable]
    public abstract class RhPort<T> : RhPort
    {
         public List<string> connectedPorts = new();
        public override Type AcceptedType => typeof(T);

        public override object GetValue()
        {
            return GetValueNoBoxing();
        }
        public override void SetValue(object value)
        {
            SetValueNoBoxing((T)value);
        }
        public abstract T GetValueNoBoxing();

        public abstract void SetValueNoBoxing(T value);
        
        public override void Connect(RhPort toPort)
        {
            connectedPorts.Add(toPort.GUID);
        }

        public override void Disconnect(RhPort port)
        {
            connectedPorts.Remove(port.GUID);
        }
        
        public override RhPort[] GetConnectedPorts()
        {
            var ports = new RhPort[connectedPorts.Count];
            for (var i = 0; i < connectedPorts.Count; i++)
            {
                ports[i] = node.tree.FindPortByGUID(connectedPorts[i]);
            }

            return ports;
        }

        public override RhPort GetPortAtIndex(int index)
        {
            var guid = connectedPorts[index];
            return node.tree.FindPortByGUID(guid);
        }

        public override int GetConnectedPortCount()
        {
            return connectedPorts.Count;
        }

        public override void ForwardValue(RhPort target)
        {
            switch (target)
            {
                case IRhSinglePort<T> singlePort:
                    singlePort.SetValueNoBoxing(GetValueNoBoxing());
                    break;
                case RhDataSourcePort<T> dataSourcePort:
                    dataSourcePort.SetValueNoBoxing(GetValueNoBoxing());
                    break;
                case RhMultiPort<T> multiPort:
                    return;
            }
        }

        public override List<string> GetConnectedPortGuids()
        {
            return connectedPorts;
        }
    }
}