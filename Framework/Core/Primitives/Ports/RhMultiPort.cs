using System;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public interface IRhMultiPort
    {
    }

    [Serializable]
    public abstract class RhMultiPort<T> : RhPort<T>, IRhMultiPort
    {
        public T value;
        
        public static implicit operator T(RhMultiPort<T> port)
        {
            return port.value;
        }
        public void ForEachConnected(Action<T> action)
        {
            foreach (var connectedPort in connectedPorts)
            {
                var val = (node.tree.FindPortByGUID(connectedPort) as RhPort<T>).GetValueNoBoxing();
                action(val);
            }
        }

        public override T GetValueNoBoxing()
        {
            return value;
        }

        public override void SetValueNoBoxing(T value)
        {
            this.value = value;
        }
    }
}