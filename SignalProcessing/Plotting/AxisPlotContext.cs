using UnityEngine;

namespace RobertHoudin.SignalProcessing.Plotting
{
    public enum AxisDirection
    {
        Horizontal,
        Vertical
    }
    public class AxisPlotContext
    {
        public bool isVisible = true;
        public RenderTexture texture;
        public float axisValue;
        public Vector2 axisRange;
        public Color color;
        public Color clearColor;
        public bool requireClear;
        public AxisDirection axisDirection;
    }
}