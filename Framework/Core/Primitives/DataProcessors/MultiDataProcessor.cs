namespace RobertHoudin.Framework.Core.Primitives.DataProcessors
{
    public abstract class MultiDataProcessor<T> : BaseMultiDataProcessor
    {
        public abstract T Process(T u, T v);
    }
    
}