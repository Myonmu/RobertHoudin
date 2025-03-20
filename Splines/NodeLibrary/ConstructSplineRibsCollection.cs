using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.NodeLibrary.Loop;
using RobertHoudin.Splines.Runtime.RhSpline;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace RobertHoudin.Splines.NodeLibrary
{
    public class ConstructSplineRibsCollection : ForEachNode<SplinePortDs,
        SplineControlPointPort, SplineRibPort, SplineRibsCollectionPort, ISplineControlPoint, SplineRib>
    {
        private IEnumerator<ISplineControlPoint> _controlPointEnumerator;

        public override void ResetNode(RhTree parent)
        {
            base.ResetNode(parent);
            _controlPointEnumerator?.Dispose();
        }
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            _controlPointEnumerator = collectionInput.GetValueNoBoxing().GetControlPoints();
            try
            {
                base.OnEvaluate(context);
            }
            finally
            {
                _controlPointEnumerator.Dispose();
            }
            return true;
        }
        protected override int GetInputCollectionSize(SplinePortDs port)
        {
            return port.GetValueNoBoxing().ControlPointCount;
        }
        protected override ISplineControlPoint Extract(SplinePortDs input, int i)
        {
            _controlPointEnumerator.MoveNext();
            return _controlPointEnumerator.Current;
        }
        protected override void Put(SplineRibsCollectionPort outputPort, int i, SplineRib value)
        {
            outputPort.GetValueNoBoxing().Add(value);
        }
    }
}