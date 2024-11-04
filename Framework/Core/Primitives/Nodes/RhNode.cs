using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public abstract class RhNode : ScriptableObject
    {
        public virtual List<RhPort> InputPorts => new();
        public virtual List<RhPort> OutputPorts => new();
        
        [HideInInspector] public RhNodeStatus status;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;
        [HideInInspector] public string info;
        [HideInInspector] public string subInfo;
        public virtual string Tooltip { get; }

        public void EvaluateNode(Agent agent, Blackboard blackboard)
        {
            if (status is not RhNodeStatus.Idle)
            {
                return;
            }

            status = RhNodeStatus.WaitingForDependency;
            OnBeginEvaluate(agent, blackboard);
            // requires all parent nodes to finish evaluating
            foreach (var inputPort in InputPorts)
            {
                foreach (var port in inputPort.GetConnectedPorts())
                {
                    port.node.EvaluateNode(agent, blackboard);
                }
            }
            status = OnEvaluate(agent, blackboard) ? RhNodeStatus.Success : RhNodeStatus.Failed;
        }

        public virtual RhNode Clone()
        {
            return Instantiate(this);
        }

        public virtual void ResetNode()
        {
            status = RhNodeStatus.Idle;
        }

        protected abstract void OnBeginEvaluate(Agent agent, Blackboard blackboard);
        protected abstract bool OnEvaluate(Agent agent, Blackboard blackboard);

        public virtual void UpdateInfo()
        {
        }

        public virtual void AssignParent(RhNode parentNode)
        {
        }

        public virtual RhNode GetParent()
        {
            return null;
        }
    }
}