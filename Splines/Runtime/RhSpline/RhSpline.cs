using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Interpolation;
using RobertHoudin.Splines.Runtime.CustomData;
using RobertHoudin.Splines.Runtime.SplineInterface;
using UnityEngine;
using UnityEngine.Assertions;
namespace RobertHoudin.Splines.Runtime.RhSpline
{
    public abstract class RhSpline
    {
        public abstract OrderedControlPointCollection OrderedControlPoints { get; }
        public virtual void OnBeginEvaluate(){}
    }
    /// <summary>
    /// Augments a basic spline with custom interpolators,
    /// so that they can be used with spline tools that require custom data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RhSpline<T> : RhSpline,
        IBasicSpline, ISplineWithLateralRotation, ISplineWithWidth, ISplineWithCustomData
        where T : ISplineWithPosition, ISplineWithTangent, ISplineWithNormal
    {
        public T rawSpline;
        private OrderedControlPointCollection _orderedControlPoints;
        public RhSplineCustomDataCollection customDataCollection;
        public Vector3 EvaluatePosition(float t)
        {
            return rawSpline.EvaluatePosition(t);
        }
        public Vector3 EvaluateTangent(float t)
        {
            return rawSpline.EvaluateTangent(t);
        }
        public Vector3 EvaluateUp(float t)
        {
            return rawSpline.EvaluateUp(t);
        }
        public float EvaluateLateralRotation(float t)
        {
            if (rawSpline is ISplineWithLateralRotation splineWithLateralRotation)
                return splineWithLateralRotation.EvaluateLateralRotation(t);
            if (rawSpline is ISplineWithCustomData splineWithCustomData) 
                return splineWithCustomData.Evaluate<float>("roll", t);

            Assert.IsTrue(customDataCollection != null, "Must provide custom data collection for interp.");
            _orderedControlPoints ??= new OrderedControlPointCollection(rawSpline.GetControlPoints());
            try
            {
                return Interpolate<float>("roll", t);
            }
            catch
            {
                return 0;
            }
        }
        public float EvaluateWidth(float t)
        {
            if (rawSpline is ISplineWithWidth splineWithWidth) return splineWithWidth.EvaluateWidth(t);
            if (rawSpline is ISplineWithCustomData splineWithCustomData) return splineWithCustomData.Evaluate<float>("width", t);
            
            Assert.IsTrue(customDataCollection != null, "Must provide custom data collection for interp.");
            _orderedControlPoints ??= new OrderedControlPointCollection(rawSpline.GetControlPoints());
            return Interpolate<float>("width", t);
        }
        
        float ISplineWithWidth.EvaluateWidthEccentricity(float t)
        {
            if (rawSpline is ISplineWithWidth splineWithWidth) return splineWithWidth.EvaluateWidthEccentricity(t);
            if (rawSpline is ISplineWithCustomData splineWithCustomData) return splineWithCustomData.Evaluate<float>("eccentricity", t);

            Assert.IsTrue(customDataCollection != null, "Must provide custom data collection for interp.");
            _orderedControlPoints ??= new OrderedControlPointCollection(rawSpline.GetControlPoints());
            try
            {
                return Interpolate<float>("eccentricity", t);
            }
            catch
            {
                return 0f;
            }
        }
        public T1 Interpolate<T1>(string key, float t)
        {
            var entry = customDataCollection.customDataList.First(x => x.data.dataKey == key);
            if(entry.interpolator is not IInterpolatorT<T1> interpolator) 
                throw new Exception("Incompatible interpolator");
            if(entry.data is not RhSplineCustomData<T1> data) throw new Exception($"Incompatible data collection type");
            t = data.SearchClosestPointsWithData(OrderedControlPoints, t, 
                out _, out _, out var before, out var after);
            return interpolator.Interpolate(before, after, t);
        }
        public Vector3 ReferencePoint => rawSpline.ReferencePoint;
        public IEnumerator<ISplineControlPoint> GetControlPoints()
        {
            return rawSpline.GetControlPoints();
        }
        public int ControlPointCount => rawSpline.ControlPointCount;
        public T1 Evaluate<T1>(string dataKey, float t)
        {
            return Interpolate<T1>(dataKey, t);
        }
        public override OrderedControlPointCollection OrderedControlPoints
        {
            get => _orderedControlPoints;
        }
    }
}