using System;
using System.Collections.Generic;
using RobertHoudin.SignalProcessing.Plotting;
using UnityEngine;

namespace RobertHoudin.SignalProcessing.Oscilloscope
{
    public class AutoEvalOscilloscopeGraph : OscilloscopeGraph
    {
        private List<Func<float, float>> _evalFuncs;
        private Func<float> _timeGetter;
        private LinSpaceF _timeLinSpace;
        private AxisPlotContext timeSweep;
        public bool OriginIsCurrentTime = false;

        public AutoEvalOscilloscopeGraph(
            int width, int height, float timeSpan, float resolution,
            Func<float> timeGetter,
            params Func<float, float>[] evalFuncs) : base(width, height)
        {
            _timeGetter = timeGetter;
            _timeLinSpace = new LinSpaceF(timeSpan, 1 / resolution);
            _evalFuncs = new List<Func<float, float>>();
            _evalFuncs.AddRange(evalFuncs);
            timeSweep = new()
            {
                requireClear = false,
                texture = texture,
                axisDirection = AxisDirection.Vertical,
                color = Color.blue
            };
        }

        public RenderTexture Texture => texture;

        public void SetTimeParams(float timespan, float resolution)
        {
            _timeLinSpace.end = timespan;
            _timeLinSpace.step = 1 / resolution;
        }

        public void Eval()
        {
            using var timeEnumerator = _timeLinSpace.GetEnumerator();
            var counter = 0;
            while (timeEnumerator.MoveNext())
            {
                for (int i = 0; i < _evalFuncs.Count; i++)
                {
                    if (!GetPlotVisibility(i)) continue;
                    this[i, counter] = new Vector2(timeEnumerator.Current, _evalFuncs[i]
                        .Invoke(timeEnumerator.Current + (OriginIsCurrentTime ? _timeGetter.Invoke() : 0)));
                }

                counter++;
            }
        }

        public override void Plot()
        {
            base.Plot();
            if (!OriginIsCurrentTime)
            {
                timeSweep.axisRange = new Vector2(Bounds.x, Bounds.z);
                timeSweep.axisValue =
                    Time.time - Mathf.Floor(_timeGetter.Invoke() / _timeLinSpace.end) * _timeLinSpace.end;
                GraphPlot.PlotAxis(timeSweep);
            }
        }
    }
}