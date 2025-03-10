using RobertHoudin.Splines.Runtime;
using RobertHoudin.Splines.Runtime.SplineInterface;
namespace Plugins.RobertHoudin.Splines.Editor
{
    public static class RhSplineDrawer
    {
        public static ISpline TargetSpline;
        public static ISplineControlPoint TargetControlPoint;
        public static event System.Action<ISpline, int, ISplineControlPoint> OnTargetControlPointChanged;
        public static event System.Action<ISpline> OnTargetSplineChanged;
        
        
    }
}