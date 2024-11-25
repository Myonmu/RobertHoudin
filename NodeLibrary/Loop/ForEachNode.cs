using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary.Loop
{
    public abstract class ForEachNode<InputPort, ItemPort, ItemResult, OutputPort, T, U> : RhNode
        where InputPort : RhPort
        where ItemResult : RhPort<U>
        where ItemPort : RhPort<T>
        where OutputPort : RhPort
    {
        protected bool isInLoop = false;
        [RhInputPort] public InputPort collectionInput;
        [RhLoopItemPort] public ItemPort itemPort;
        [RhLoopResultPort] public ItemResult itemResult;
        [RhOutputPort] public OutputPort collectionOutput;
        private List<RhPort> _outputPortsGenericCache;
        private List<RhPort> _inputPortsGenericCache;

        public override List<RhPort> OutputPortsGeneric
        {
            get
            {
                if (_outputPortsGenericCache == null)
                {
                    _outputPortsGenericCache = this.GetPortsWithAttribute<RhOutputPortAttribute>();
                    _outputPortsGenericCache.Add(itemPort);
                }
                return _outputPortsGenericCache;
            }
        }

        public override List<RhPort> InputPortsGeneric
        {
            get
            {
                if (_inputPortsGenericCache == null)
                {
                    _inputPortsGenericCache = this.GetPortsWithAttribute<RhInputPortAttribute>();
                    _inputPortsGenericCache.Add(itemResult);
                }
                return _inputPortsGenericCache;
            }
        }
        public override void EvaluateNode(RhExecutionContext context, EvalMode mode = EvalMode.Normal)
        {
            if (mode is EvalMode.Loop || status is not RhNodeStatus.Idle)
            {
                return;
            }

            status = RhNodeStatus.WaitingForDependency;
            OnBeginEvaluate(context);
            for (var i = 0; i < collectionInput.GetConnectedPortCount(); i++)
            {
                if (collectionInput.GetPortAtIndex(i) == null) continue;
                collectionInput.GetPortAtIndex(i).node.EvaluateNode(context);
                collectionInput.GetPortAtIndex(i).ForwardValue(collectionInput);
                if (collectionInput.GetPortAtIndex(i).node.status is RhNodeStatus.Failed)
                {
                    status = RhNodeStatus.Failed;
                    return;
                }
            }

            // eval per-item
            status = OnEvaluate(context) ? RhNodeStatus.Success : RhNodeStatus.Failed;
        }

        protected abstract int GetInputCollectionSize(InputPort port);

        protected abstract T Extract(InputPort input, int i);
        protected abstract void Put(OutputPort outputPort, int i, U value);

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            // todo: stub
            for (int i = 0; i < GetInputCollectionSize(collectionInput); i++)
            {
                itemPort.SetValueNoBoxing(Extract(collectionInput, i));
                foreach (var connectedPort in itemResult.GetConnectedPorts())
                {
                    connectedPort.node.EvaluateNode(context, EvalMode.Loop);
                    connectedPort.ForwardValue(itemResult);
                }
                Put(collectionOutput, i, itemResult.GetValueNoBoxing());
            }
            return true;
        }

        public override RhPort GetOutputPortByGUID(string guid)
        {
            if (itemPort.GUID == guid) return itemPort;
            return base.GetOutputPortByGUID(guid);
        }

        public override RhPort GetInputPortByGUID(string guid)
        {
            if (itemResult.GUID == guid) return itemResult;
            return base.GetInputPortByGUID(guid);
        }
    }
}