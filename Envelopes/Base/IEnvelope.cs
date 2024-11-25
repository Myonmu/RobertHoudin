namespace TATools.RobertHoudin
{
    /// <summary>
    /// Envelop is a curve/function f(x): [0,1] -> R
    /// </summary>
    public interface IEnvelope
    {
        /// <summary>
        /// Whether this envelope is activated.
        /// Inactive envelop would 
        /// </summary>
        public bool IsActive { get;}
        public float Evaluate(float i);
        /// <summary>
        /// Some modules may use this to check if an envelope has changed
        /// and thus requires a repaint. They would also consume the dirty
        /// flag (set it to false).
        /// </summary>
        public bool IsDirty { get; set; }
    }
}