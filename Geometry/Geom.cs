using System;
namespace RobertHoudin.Geometry
{
    public static class Geom
    {
        public static void SplitLineSegmentNoAlloc<TPoint, TDistanceMetric>(
            IParameterizedLineSegment<TPoint, TDistanceMetric> segment, 
            ref TPoint[] points, int writeStartIndex = 0,  int divisions = 1)
        {
            if (points == null || points.Length <= writeStartIndex + divisions + 1)
            {
                throw new Exception("Points array is null or has no enough space");
            }
            var a = segment.A;
            var b = segment.B;
            
            var vs = segment.VectorSpace;
            if(vs == null) throw new Exception("VectorSpace is null");
            var sum = vs.Sum(a, b);
            var cursor = writeStartIndex;
            for (int i = 0; i <= divisions; i++)
            {
                points[cursor] = segment.Evaluate(i / (float)divisions);
                cursor++;
            }
        }
        
    }
}