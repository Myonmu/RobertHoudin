using System;
using System.Collections.Generic;
using RobertHoudin.Interpolation;
using UnityEngine;
namespace RobertHoudin.Splines.Runtime.CustomData
{
    public class RhSplineCustomDataCollection: MonoBehaviour
    {
        [Serializable]
        public class RhSplineCustomDataCollectionEntry
        {
            [SerializeReference] public RhSplineCustomData data;
            [SerializeReference] public IInterpolator interpolator;
        }
        public List<RhSplineCustomDataCollectionEntry> customDataList = new();
    }
}