using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public abstract class RhNode : ScriptableObject
    {
        [field: SerializeReference]
        public virtual List<RhPort> InputPorts => new();
        [field: SerializeReference]
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
                    if (port == null) continue;
                    port.node.EvaluateNode(agent, blackboard);
                    port.ForwardValue(inputPort);
                    if (port.node.status is RhNodeStatus.Failed)
                    {
                        status = RhNodeStatus.Failed;
                        return;
                    }
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
            foreach (var inputPort in InputPorts)
            {
                if(inputPort == null)continue;
                inputPort.node = this;
            }
            foreach (var outputPort in OutputPorts)
            {
                if(outputPort == null)continue;
                outputPort.node = this;
            }
            status = RhNodeStatus.Idle;
        }

        protected virtual void OnBeginEvaluate(Agent agent, Blackboard blackboard){}
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

        public T GetInputValueAtPort<T>(int index)
        {
            return (T)InputPorts[index].GetValue();
        }

        public void SetOutputValueAtPort(int index, object value)
        {
            OutputPorts[index].SetValue(value);
        }

        public RhPort GetOutputPortByGUID(string guid)
        {
            return OutputPorts.FirstOrDefault(x => x != null && x.GUID == guid);
        }
        public RhPort GetInputPortByGUID(string guid)
        {
            return InputPorts.FirstOrDefault(x => x != null && x.GUID == guid);
        }
        
    }
}