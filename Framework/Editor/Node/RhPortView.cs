using System;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace RobertHoudin.Framework.Editor.Node
{
    public class RhPortView: Port
    {
        internal bool isMissingConnection = false;
        public static RhPortView CreateRhPortView<TEdge>(
            Orientation orientation,
            Direction direction,
            Port.Capacity capacity,
            System.Type type)
            where TEdge : Edge, new()
        {
            var listener = new RhEdgeConnectorListener();
            var ele = new RhPortView(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            ele.AddManipulator(ele.m_EdgeConnector);
            return ele;
        }
        protected RhPortView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
            if (type == typeof(Number))
            {
                portColor = new Color(1f, 0.7f, 0.5f);
            }
        }
    }
}