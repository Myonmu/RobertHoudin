using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;
namespace RobertHoudin.Scatter.NodeLibrary.Misc
{
    public class ExtractTransformChildrenPoints: RhNode
    {
        public DataSource<Transform> root;
        [RhOutputPort] public Vector2CollectionPort output;

        public static List<Vector2> Extract(Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>()
                .Select(x => new Vector2(x.position.x, x.position.z)).ToList();
        }
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var transform = root.GetValue(context, this);
            if (transform == null) return false;
            output.SetValueNoBoxing(Extract(transform));
            return true;
        }
    }
}