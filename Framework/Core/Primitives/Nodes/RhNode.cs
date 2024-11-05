using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public abstract class RhNode : ScriptableObject
    {
        private List<RhPort> _inputPortsCache = null;

        public virtual List<RhPort> InputPorts
        {
            get
            {
                _inputPortsCache ??= GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<RhInputPortAttribute>() is not null)
                    .Select(x => x.GetValue(this) as RhPort).ToList();
                return _inputPortsCache;
            }
        }

        private List<RhPort> _outputPortsCache = null;

        public virtual List<RhPort> OutputPorts
        {
            get { _outputPortsCache ??= GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<RhOutputPortAttribute>() is not null)
                    .Select(x => x.GetValue(this) as RhPort).ToList();
                return _outputPortsCache;}
        }

        [HideInInspector] public RhNodeStatus status;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;
        [HideInInspector] public string info;
        [HideInInspector] public string subInfo;
        public virtual string Tooltip { get; }

        public void EvaluateNode(RhExecutionContext context)
        {
            if (status is not RhNodeStatus.Idle)
            {
                return;
            }

            status = RhNodeStatus.WaitingForDependency;
            OnBeginEvaluate(context);
            // requires all parent nodes to finish evaluating
            foreach (var inputPort in InputPorts)
            {
                foreach (var port in inputPort.GetConnectedPorts())
                {
                    if (port == null) continue;
                    port.node.EvaluateNode(context);
                    port.ForwardValue(inputPort);
                    if (port.node.status is RhNodeStatus.Failed)
                    {
                        status = RhNodeStatus.Failed;
                        return;
                    }
                }
            }

            status = OnEvaluate(context) ? RhNodeStatus.Success : RhNodeStatus.Failed;
        }

        public virtual RhNode Clone()
        {
            return Instantiate(this);
        }

        public virtual void ResetNode()
        {
            foreach (var inputPort in InputPorts)
            {
                if (inputPort == null) continue;
                inputPort.node = this;
            }

            foreach (var outputPort in OutputPorts)
            {
                if (outputPort == null) continue;
                outputPort.node = this;
            }

            status = RhNodeStatus.Idle;
        }

        protected virtual void OnBeginEvaluate(RhExecutionContext context)
        {
        }

        protected abstract bool OnEvaluate(RhExecutionContext context);

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