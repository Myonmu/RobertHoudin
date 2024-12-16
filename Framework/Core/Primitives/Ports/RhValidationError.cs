namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public enum RhValidationError
    {
        None,
        /// <summary>
        /// Using reflection but binding is missing (empty field name)
        /// </summary>
        MissingReflectionBinding,
        /// <summary>
        /// Using port connection and nothing is connected
        /// </summary>
        MissingConnection
    }
}