namespace RobertHoudin.Framework.Core.Primitives.DataProcessors
{
    public abstract class DataProcessor<T> : BaseDataProcessor
    {
        public abstract T Process(T o);
    }
}