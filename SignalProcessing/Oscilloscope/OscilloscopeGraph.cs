using System;
using System.Collections.Generic;
using RobertHoudin.SignalProcessing.Plotting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace RobertHoudin.SignalProcessing.Oscilloscope
{
    /// <summary>
    /// An orthogonal scatter plot using separate X and Y datasets
    /// </summary>
    public class OscilloscopeGraph : IDisposable
    {
        protected RenderTexture texture;

        public Vector4 Bounds { get; private set; } = new Vector4(
            float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public List<GraphPlotContext> plotContexts = new();
        private AxisPlotContext _xAxisPlotContext;
        private AxisPlotContext _yAxisPlotContext;

        public Vector2 this[int plotNum, int i]
        {
            get { return plotContexts[plotNum].dataPoints[i]; }
            set
            {
                EnsurePlotContextSize(plotNum);
                var pts = plotContexts[plotNum].dataPoints;
                if (pts.Count <= i) EnsureSize(pts, i);
                pts[i] = value;
                var b = Bounds;
                if (value.x < Bounds.x) b.x = value.x;
                else if (value.x > Bounds.z) b.z = value.x;
                if (value.y < Bounds.y) b.y = value.y;
                else if (value.y > Bounds.w) b.w = value.y;
                Bounds = b;
            }
        }

        public void EnsureSize(List<Vector2> pts, int i)
        {
            while (pts.Count < i + 1)
            {
                pts.Add(Vector2.zero);
            }
        }

        public void FitGraph()
        {
            var b = new Vector4(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity,
                float.NegativeInfinity);
            foreach (var plotContext in plotContexts)
            {
                if(!plotContext.isVisible)continue;
                for (int i = 0; i < plotContext.dataPoints.Count; i++)
                {
                    var value = plotContext.dataPoints[i];
                    if (value.x < b.x) b.x = value.x;
                    else if (value.x > b.z) b.z = value.x;
                    if (value.y < b.y) b.y = value.y;
                    else if (value.y > b.w) b.w = value.y;
                }
            }
            Bounds = b;
        }

        public OscilloscopeGraph(int width, int height)
        {
            texture = new RenderTexture(width, height, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.None)
                { enableRandomWrite = true };
            _xAxisPlotContext = new()
            {
                axisValue = 0,
                clearColor = Color.black,
                color = Color.red,
                requireClear = true,
                texture = texture,
                axisDirection = AxisDirection.Horizontal
            };
            _yAxisPlotContext = new()
            {
                axisValue = 0,
                color = Color.green,
                requireClear = false,
                texture = texture,
                axisDirection = AxisDirection.Vertical
            };
        }

        public virtual void Plot()
        {
            _xAxisPlotContext.axisRange = new Vector2(Bounds.y, Bounds.w);
            _yAxisPlotContext.axisRange = new Vector2(Bounds.x, Bounds.z);
            GraphPlot.PlotAxis(_xAxisPlotContext);
            GraphPlot.PlotAxis(_yAxisPlotContext);
            foreach (var context in plotContexts)
            {
                context.requireClear = false;
                context.graphRange = Bounds;
                GraphPlot.Plot(context);
            }
        }

        private void EnsurePlotContextSize(int plotNum)
        {
            while (plotContexts.Count <= plotNum)
            {
                plotContexts.Add(new GraphPlotContext()
                {
                    requireClear = false,
                    texture = texture,
                    plotColor = Color.white,
                    dataPoints = new()
                });
            }
        }

        public void SetPlotColor(int plotNum, Color color)
        {
            EnsurePlotContextSize(plotNum);
            plotContexts[plotNum].plotColor = color;
        }

        public void SetPlotVisibility(int plotNum, bool visibility)
        {
            EnsurePlotContextSize(plotNum);
            plotContexts[plotNum].isVisible = visibility;
        }

        public bool GetPlotVisibility(int plotNum)
        {
            EnsurePlotContextSize(plotNum);
            return plotContexts[plotNum].isVisible;
        }


        public void Dispose()
        {
            if(texture!=null)texture.Release();
        }
    }
}