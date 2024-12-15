using System.Collections.Generic;
using RobertHoudin.Scatter.ScatterDataConstruction;
using UnityEngine;
namespace RobertHoudin.Scatter
{
    public interface IScatterPipeline
    {
        public List<Vector2> PlanarResults { get; }
        public List<float> Heights { get; }
    }

    public interface IScatterConfigChunk
    {
    }

    public interface IScatterPipelineComponent
    {
        public void UpdateConfig(IScatterConfigChunk configChunk);
    }

    public interface ISpatialScatterer : IScatterPipelineComponent
    {
        public void Scatter(ref List<Vector2> planarResults, ref List<float> heightResults);
    }

    public interface IPlanarScatterer : ISpatialScatterer
    {
        void ISpatialScatterer.Scatter(ref List<Vector2> planarResults, ref List<float> heightResults)
        {
            Scatter(ref planarResults);
        }

        void Scatter(ref List<Vector2> resultsCollection);
    }

    public interface IScatterAugmenter : IScatterPipelineComponent
    {
        public ScatterData AugmentToScatterData(Vector2 data, float height, int i, int count);
    }

    public interface IPlanarScatterAugmenter : IScatterAugmenter
    {
        ScatterData AugmentToScatterData(Vector2 data, int i, int count);

        ScatterData IScatterAugmenter.AugmentToScatterData(Vector2 data, float height, int i, int count)
        {
            return AugmentToScatterData(data, i, count);
        }
    }

    public interface IScatterDataPreprocessor : IScatterPipelineComponent
    {
        public bool Enabled { get; }
        public void ProcessScatterData(ref ScatterData scatterData, int i, ref IFilterContext ctx);
    }

    public interface IScatterDataFilter : IScatterPipelineComponent
    {
        public bool Enabled { get; }
        public bool IsScatterResultAccepted(ScatterData scatterData, IFilterContext ctx);
    }

    public interface IObjectPlacer : IScatterPipelineComponent
    {
        public void PlaceObject(in ScatterData data, in int objectRef);
    }

    public interface IObjectProvider
    {
        public GameObject GetObjectByIndex(int objectId);
        int MinIndex { get; }
        int MaxIndex { get; }
    }

    public interface IFilterContext
    {
        IScatterPipeline Pipeline { get; set; }
        public IFilterContext Create();
    }
}