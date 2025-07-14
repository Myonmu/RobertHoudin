using UnityEngine;

namespace RobertHoudin.SignalProcessing.Plotting
{
    public static class GraphPlot
    {
        private static ComputeShader _plotter;
        private static readonly int Graph = Shader.PropertyToID("_Graph");
        private static readonly int ClearColor = Shader.PropertyToID("_ClearColor");
        private static readonly int PlotColor = Shader.PropertyToID("_PlotColor");
        private static readonly int GraphBounds = Shader.PropertyToID("_GraphBounds");
        private static readonly int DataPointCount = Shader.PropertyToID("_DataPointCount");
        private static readonly int OrderedRawDataPoints = Shader.PropertyToID("_OrderedRawDataPoints");
        
        private static GraphicsBuffer _plotBuffer;
        private static bool _sharedBufferInUse;
        private static int _reservedSize = 0;
        private static readonly int AxisRange = Shader.PropertyToID("_AxisRange");
        private static readonly int RequireClear = Shader.PropertyToID("_RequireClear");
        private static readonly int AxisValue = Shader.PropertyToID("_AxisValue");
        private static readonly int Direction = Shader.PropertyToID("_AxisDirection");


        public static void Plot(GraphPlotContext ctx)
        {
            if (!ctx.isVisible) return;
            if (ctx.dataPoints.Count < 1) return;
            if (_plotter == null) _plotter = Resources.Load<ComputeShader>("GraphPlotter");
            if (_reservedSize < ctx.dataPoints.Count)
            {
                _reservedSize = Mathf.NextPowerOfTwo(ctx.dataPoints.Count);
                _plotBuffer?.Dispose();
                _plotBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, GraphicsBuffer.UsageFlags.None,
                    _reservedSize, sizeof(float) * 2);
            }
            var plotKernel =  _plotter.FindKernel("Plot");
            _plotter.SetTexture(plotKernel, Graph, ctx.texture);
            _plotter.SetVector(ClearColor, ctx.clearColor);
            _plotter.SetVector(PlotColor, ctx.plotColor);
            _plotter.SetVector(GraphBounds, ctx.graphRange);
            _plotter.SetInt(DataPointCount, ctx.dataPoints.Count);
            _plotter.SetBool(RequireClear, ctx.requireClear);
            _plotBuffer.SetData(ctx.dataPoints);
            _plotter.SetBuffer(plotKernel, OrderedRawDataPoints, _plotBuffer);
            _plotter.Dispatch(plotKernel, ctx.texture.width/32 + 1,1,1);
        }

        public static void PlotAxis(AxisPlotContext ctx)
        {
            if (!ctx.isVisible) return;
            if (_plotter == null) _plotter = Resources.Load<ComputeShader>("GraphPlotter");
            var plotKernel =  _plotter.FindKernel("PlotAxis");
            _plotter.SetTexture(plotKernel, Graph, ctx.texture);
            _plotter.SetFloat(AxisValue, ctx.axisValue);
            _plotter.SetVector(PlotColor, ctx.color);
            _plotter.SetVector(AxisRange, ctx.axisRange);
            _plotter.SetVector(ClearColor, ctx.clearColor);
            _plotter.SetBool(RequireClear, ctx.requireClear);
            _plotter.SetInt(Direction, (int)ctx.axisDirection);
            _plotter.Dispatch(plotKernel, ctx.texture.width/8 + 1,ctx.texture.height/8 + 1,1);
        }
    }
}