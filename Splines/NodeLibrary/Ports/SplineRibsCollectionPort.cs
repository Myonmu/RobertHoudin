using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Splines.Runtime.RhSpline;
namespace RobertHoudin.Splines.NodeLibrary
{
    [Serializable]
    public class SplineRibPort: RhSinglePort<SplineRib>{}
    [Serializable]
    public class SplineRibsCollectionPort: RhSinglePort<List<SplineRib>>{}
}