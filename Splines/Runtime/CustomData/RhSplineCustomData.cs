using System;
using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
using RobertHoudin.Utils.RuntimeCompatible;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.CustomData
{
    [Serializable]
    public abstract class RhSplineCustomData
    {
        public string dataKey;
        public abstract void Add(int id, object data);
        public abstract void PushCandidate();
        public abstract bool Contains(int id);

        public abstract int GetNthDataId(int n);
        public abstract void Remove(int id);
    }

    [Serializable]
    public class RhSplineCustomData<T> : RhSplineCustomData
    {
        [Serializable]
        public class RhSplineCustomDataEntry
        {
            public int controlPointIndex;
            public T data;
        }

        public RhSplineCustomDataEntry candidateData = new();
        public List<RhSplineCustomDataEntry> entries = new();

        private int InterpretControlPointIndex(int controlPointCount, int controlPointIndex)
        {
            return controlPointIndex >= 0 ? controlPointIndex : controlPointCount + controlPointIndex;
        }
        public float SearchClosestPointsWithData(OrderedControlPointCollection points, float t,
            out ISplineControlPoint beforePoint, out ISplineControlPoint afterPoint,
            out T beforeData, out T afterData)
        {
            points.SearchClosestControlPointsIndices(t, out var before, out var after);

            ClampSearch.FindClampingIndices(entries,
                e => InterpretControlPointIndex(points.Count, e.controlPointIndex),
                before, out var actualBefore, out _);
            ClampSearch.FindClampingIndices(entries,
                e => InterpretControlPointIndex(points.Count, e.controlPointIndex),
                after, out _, out var actualAfter, ClampSearchMode.StrictLowerBound);

            var i = InterpretControlPointIndex(points.Count, entries[actualBefore].controlPointIndex);
            var j = InterpretControlPointIndex(points.Count, entries[actualAfter].controlPointIndex);
            beforePoint = points.GetControlPoint(i);
            afterPoint = points.GetControlPoint(j);
            beforeData = entries[actualBefore].data;
            afterData = entries[actualAfter].data;
            var beforeDis = points.GetDistance(i);
            var afterDis = points.GetDistance(j);
            var evalDis = points.ConvertToActualDistance(t);
            return (evalDis - beforeDis) / (afterDis - beforeDis);
        }
        public override void Add(int id, object data)
        {
            entries.Add(new()
            {
                controlPointIndex = id,
                data = (T)data
            });
        }

        public override void PushCandidate()
        {
            Add(candidateData.controlPointIndex, candidateData.data);
        }

        public void Sort()
        {
            var cnt = entries.Count;
            entries.Sort(
                (a, b) =>
                    (InterpretControlPointIndex(cnt, a.controlPointIndex) -
                     InterpretControlPointIndex(cnt, b.controlPointIndex))
            );
        }

        public override bool Contains(int controlPointIndex)
        {
            return entries.Any(x => x.controlPointIndex == controlPointIndex);
        }
        public override int GetNthDataId(int n)
        {
            return entries[n].controlPointIndex;
        }
        public override void Remove(int id)
        {
            entries.RemoveAll(x => x.controlPointIndex == id);
        }
    }

    [Serializable] public class RhSplineCustomFloatData : RhSplineCustomData<float>
    {
    }
    [Serializable] public class RhSplineCustomIntData : RhSplineCustomData<int>
    {
    }
    [Serializable] public class RhSplineCustomVector3Data : RhSplineCustomData<Vector3>
    {
    }
    [Serializable] public class RhSplineCustomVector2Data : RhSplineCustomData<Vector2>
    {
    }
}