namespace RobertHoudin.Geometry
{
    public interface ILineSegment<TPoint, TLengthMetric>
    {
        public IVectorSpace<TPoint, TLengthMetric> VectorSpace { get; }
        public TPoint A { get; set; }
        public TPoint B { get; set; }
        public TLengthMetric Length { get;}
    }
}