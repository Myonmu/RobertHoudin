using System;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public interface IRhSinglePort
    {
        public object GetValue();
        public void SetValue(object value);
    }

    public interface IRhSinglePort<T> : IRhSinglePort
    {
        public T GetValueNoBoxing();
        public void SetValueNoBoxing(T value);
    }

    /// <summary>
    /// Port that can only accept one input.
    /// Note that this is best suited for value types (types that can't be assigned null).
    /// Use <see cref="RhSinglePortObjectType{T}"/> for object types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// Single Port but calls default constructor (new())
    /// </summary>
    /// <typeparam name="T"></typeparam>
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