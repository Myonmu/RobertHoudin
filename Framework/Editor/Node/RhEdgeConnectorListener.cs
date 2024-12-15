using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace RobertHoudin.Framework.Editor.Node
{
    /// <summary>
    /// Duplicate of the default edge connector listener
    /// </summary>
    public class RhEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange _graphViewChange;
        private List<Edge> _edgesToCreate;
        private List<GraphElement> _edgesToDelete;

        public RhEdgeConnectorListener()
        {
            this._edgesToCreate = new List<Edge>();
            this._edgesToDelete = new List<GraphElement>();
            this._graphViewChange.edgesToCreate = this._edgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            this._edgesToCreate.Clear();
            this._edgesToCreate.Add(edge);
            this._edgesToDelete.Clear();
            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                        this._edgesToDelete.Add(connection);
                }
            }
            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge)
                        this._edgesToDelete.Add(connection);
                }
            }
            if (this._edgesToDelete.Count > 0)
                graphView.DeleteElements(this._edgesToDelete);
            List<Edge> edgesToCreate = this._edgesToCreate;
            if (graphView.graphViewChanged != null)
                edgesToCreate = graphView.graphViewChanged(this._graphViewChange).edgesToCreate;
            foreach (Edge edge1 in edgesToCreate)
            {
                graphView.AddElement(edge1);
                edge.input.Connect(edge1);
                edge.output.Connect(edge1);
            }
        }
    }
}