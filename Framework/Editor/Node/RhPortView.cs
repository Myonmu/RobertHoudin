using System;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using RobertHoudin.Framework.Editor.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
namespace RobertHoudin.Framework.Editor.Node
{
    public class RhPortView: Port
    {
        private Color _naturalColor;
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
                _naturalColor = new Color(1f, 0.7f, 0.5f);
            }else if (type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4))
            {
                _naturalColor = Color.yellow;
            }else if (type == typeof(Quaternion))
            {
                _naturalColor = new Color(0.6f, 0.45f, 1);
            }else if (type == typeof(int))
            {
                _naturalColor = new Color(0, 0.85f, 0.3f);
            }else if (type == typeof(float))
            {
                _naturalColor = Color.cyan;
            }else if (type == typeof(Color))
            {
                _naturalColor = new Color(0.9f, 0.3f, 0.6f);
            }
            else
            {
                _naturalColor = Color.gray;
            }
            portColor = _naturalColor;
        }

        public void Validate(RhPort port)
        {
            // don't care about output consummation
            if (direction == Direction.Output) return;
            if (port is IDataSourcePort ds)
            {
                switch (ds.SourceType)
                {
                    case SourceType.None:
                        portColor = Color.black;
                        return;
                    case SourceType.PropertyBlock:
                        portColor = string.IsNullOrEmpty(ds.SourceName) ? Color.red : Color.black;
                        return;
                    case SourceType.Port:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            portColor = port.GetConnectedPortCount() == 0 ? Color.magenta: _naturalColor;
        }
    }
}