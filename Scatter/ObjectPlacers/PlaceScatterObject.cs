using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Scatter.ObjectProviders;
using RobertHoudin.Scatter.ScatterDataConstruction;
using UnityEditor;
using UnityEngine;
namespace RobertHoudin.Scatter.ObjectPlacers
{
    public class PlaceScatterObject: RhNode
    {

        [RhInputPort] public ScatterDataCollectionPort scatterDataCollection;
        [RhInputPort] public TransformPortDs rootTransform;
        [RhInputPort] public ObjectProviderPortDs objectProvider;
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            foreach (var data in scatterDataCollection.GetValueNoBoxing())
            {
                var instance = PrefabUtility.InstantiatePrefab(
                    objectProvider.GetValueNoBoxing().GetObjectByIndex(data.objectId), 
                    rootTransform.GetValueNoBoxing()) as GameObject;
                instance.transform.position = data.pos;
                instance.transform.rotation = data.rotation;
                instance.transform.localScale = data.scale;
            }
            return true;
        }
    }
}