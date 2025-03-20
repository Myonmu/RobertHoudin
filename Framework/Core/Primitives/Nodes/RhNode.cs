using System.Collections.Generic;
using System.Linq;
using RobertHoudin.Framework.Core.Primitives.Ports;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Nodes
{
    public abstract class RhNode : ScriptableObject
    {
        #if !RH_DEBUG
        [HideInInspector]
        #endif
        public RhTree tree;
        private List<RhPort> _inputPortsCache = null;

        public virtual List<RhPort> InputPorts
        {
            get
            {
                _inputPortsCache ??= this.GetPortsWithAttribute<RhInputPortAttribute>();
                return _inputPortsCache;
            }
        }
        
        /// <summary>
        /// Used when populating view.
        /// Should include special ports (like loop input)
        /// </summary>
        public virtual List<RhPort> InputPortsGeneric => InputPorts;

        private List<RhPort> _outputPortsCache = null;

        public virtual List<RhPort> OutputPorts
        {
            get
            {
                _outputPortsCache ??= this.GetPortsWithAttribute<RhOutputPortAttribute>();
                return _outputPortsCache;
            }
        }
        
        /// <summary>
        /// Used when populating view.
        /// Should include special ports (like loop output)
        /// </summary>
        public virtual List<RhPort> OutputPortsGeneric => OutputPorts;

        public RhNodeStatus status;
        public RhNodeFlag flags;
        [SerializeField] [HideInInspector] private string _guid;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;
        [HideInInspector] public string info;
        [HideInInspector] public string subInfo;

        public string GUID
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = UnityEditor.GUID.Generate().ToString();
                }
#endif

                return _guid;
            }
        }

        public virtual string Tooltip { get; }

        public virtual void EvaluateNode(RhExecutionContext context, EvalMode mode = EvalMode.Normal)
        {
            if (mode is EvalMode.Normal && status is not RhNodeStatus.Idle)
            {
                return;
            }

            OnBeginEvaluate(context);
            status = RhNodeStatus.WaitingForDependency;
            
            // requires all parent nodes to finish evaluating
            foreach (var inputPort in InputPorts)
            {
                if(!inputPort.IsActive) continue;
                for (var i = 0 ; i < inputPort.GetConnectedPortCount(); i++)
                {
                    if (inputPort.GetPortAtIndex(i) == null) continue;
                    inputPort.GetPortAtIndex(i).node.EvaluateNode(context, mode);
                    inputPort.GetPortAtIndex(i).ForwardValue(inputPort);
                    if (inputPort.GetPortAtIndex(i).node.status is not RhNodeStatus.Failed) continue;
                    status = RhNodeStatus.Failed;
                    return;
                }
            }
            
            status = OnEvaluate(context) ? RhNodeStatus.Success : RhNodeStatus.Failed;
        }

        public virtual void ResetNode(RhTree parent)
        {
            tree = parent;
            foreach (var inputPort in InputPortsGeneric)
            {
                if (inputPort == null) continue;
                inputPort.node = this;
            }

            foreach (var outputPort in OutputPortsGeneric)
            {
                if (outputPort == null) continue;
                outputPort.node = this;
            }

            status = RhNodeStatus.Idle;
        }

        protected virtual void OnBeginEvaluate(RhExecutionContext context)
        {
            this.EvalDataSources(context); //TODO: Make this Opt-in to improve perf
        }

        protected abstract bool OnEvaluate(RhExecutionContext context);

        public virtual void UpdateInfo()
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

        public T GetOutputValueAtPort<T>(int index)
        {
            return (T)OutputPorts[index].GetValue();
        }

        public void SetOutputValueAtPort(int index, object value)
        {
            OutputPorts[index].SetValue(value);
        }

        public virtual RhPort GetOutputPortByGUID(string guid)
        {
            return OutputPorts.FirstOrDefault(x => x != null && x.GUID == guid);
        }

        public virtual RhPort GetInputPortByGUID(string guid)
        {
            return InputPorts.FirstOrDefault(x => x != null && x.GUID == guid);
        }

        public void DisconnectAll(RhTree tree)
        {
            foreach (var port in InputPortsGeneric)
            {
                foreach (var guid in port.GetConnectedPortGuids())
                {
                    var connectedPort = tree.FindPortByGUID(guid);
                    connectedPort.GetConnectedPortGuids().Remove(port.GUID);
                }
            }
            foreach (var port in OutputPortsGeneric)
            {
                foreach (var guid in port.GetConnectedPortGuids())
                {
                    var connectedPort = tree.FindPortByGUID(guid);
                    connectedPort.GetConnectedPortGuids().Remove(port.GUID);
                }
            }
        }
    }
}