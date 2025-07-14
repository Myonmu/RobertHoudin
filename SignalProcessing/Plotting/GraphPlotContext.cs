using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobertHoudin.SignalProcessing.Plotting
{
    public class GraphPlotContext: IDisposable
    {
        public bool isVisible = true;
        public RenderTexture texture;
        public Vector4 graphRange; // xMin yMin xMax yMax (rect)
        public List<Vector2> dataPoints;
        public Color clearColor = Color.clear;
        public Color plotColor = Color.magenta;
        public bool requireClear = true;

        public void Dispose()
        {
            texture.Release();
        }
    }
}