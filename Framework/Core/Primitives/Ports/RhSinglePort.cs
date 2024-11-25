using System;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public interface IRhSinglePort
    {
    }

    public interface IRhSinglePort<T> : IRhSinglePort
    {
        public T GetValueNoBoxing();
        public void SetValueNoBoxing(T value);
    }

    [Serializable]
    public abstract class RhSinglePort<T> : RhPort<T>, IRhSinglePort<T>
    {
        public T value;
        public static implicit operator T(RhSinglePort<T> port)
        {
            return port.value;
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

    public abstract class RhSinglePortObjectType<T> : RhPort<T>, IRhSinglePort<T> where T : new()
    {
        public T value = new();
        public static implicit operator T(RhSinglePortObjectType<T> port)
        {
            return port.value;
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