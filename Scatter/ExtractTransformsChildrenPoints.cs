using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEngine;

namespace RobertHoudin.Scatter
{
    public class ExtractTransformsChildrenPoints: RhNode
    {
        public DataSource<Transform> root;
        [RhOutputPort] public PointCollection2DPort output;

        public static List<Vector2> Extract(Transform transform)
        {
            return transform.GetComponentsInChildren<Transform>()
                .Select(x => new Vector2(x.position.x, x.position.z)).ToList();
        }
        
        protected override bool OnEvaluate(RhExecutionContext context)
        {
            var transform = root.GetValue(context, this);
            if (transform == null) return false;
            output.SetValueNoBoxing(new PointCollection2D(){points = Extract(transform)});
            return true;
        }
    }
}