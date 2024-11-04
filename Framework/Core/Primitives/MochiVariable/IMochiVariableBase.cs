namespace RobertHoudin.Framework.Core.Primitives.MochiVariable
{
    public interface IMochiVariableBase
    {
        public string Key { get; }
        public object BoxedValue { get; set; }
        public void InitializeBinding();
    }
}