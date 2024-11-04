namespace RobertHoudin.Framework.Core.Primitives.Events
{
    public interface ISubscribable
    {
        public void Subscribe(IListener listener);
        public void Unsubscribe(IListener listener);
    }
}