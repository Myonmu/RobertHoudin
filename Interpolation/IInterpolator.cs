namespace RobertHoudin.Interpolation
{

    public interface IInterpolator
    {
        
    }
    
    public interface IInterpolatorT<T> : IInterpolator
    {
        public T Interpolate(T from, T to, float t);
    }
}