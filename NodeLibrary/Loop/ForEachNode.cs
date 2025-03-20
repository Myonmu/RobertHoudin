using System.Collections.Generic;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary.Loop
{
    /// <summary>
    /// Run through each item in the input collection, produces output collection.
    /// Similar to LINQ's Select.
    /// </summary>
    /// <typeparam name="InputPort">type of the input collection port</typeparam>
    /// <typeparam name="ItemPort">port for each item in the input collection</typeparam>
    /// <typeparam name="ItemResult">port for each item in the output collection</typeparam>
    /// <typeparam name="OutputPort">type of the output collection port</typeparam>
    /// <typeparam name="T">input item type</typeparam>
    /// <typeparam name="U">output item type</typeparam>
    public abstract class ForEachNode<InputPort, ItemPort, ItemResult, OutputPort, T, U> : RhNode
        where InputPort : RhPort
        where ItemResult : RhPort<U>
        where ItemPort : RhPort<T>
        where OutputPort : RhPort
    {
        protected bool isInLoop = false;
        [RhInputPort] public InputPort collectionInput;
        [RhLoopItemPort] public ItemPort itemPort;
        [RhLoopItemPort] public NumberPort indexPort;
        [RhLoopResultPort] public ItemResult itemResult;
        [RhOutputPort] public OutputPort collectionOutput;
        private List<RhPort> _outputPortsGenericCache;
        private List<RhPort> _inputPortsGenericCache;
        private HashSet<RhNode> _nodesInLoop = new();

        public override List<RhPort> OutputPortsGeneric
        {
            get {
                if (_outputPortsGenericCache == null)
                {
                    _outputPortsGenericCache = this.GetPortsWithAttribute<RhOutputPortAttribute>();
                    _outputPortsGenericCache.Add(itemPort);
                    _outputPortsGenericCache.Add(indexPort);
                }
                return _outputPortsGenericCache;
            }
        }

        public override List<RhPort> InputPortsGeneric
        {
            get {
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
            _nodesInLoop.Clear();
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

        /// <summary>
        /// Extract the size of the input collection
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        protected abstract int GetInputCollectionSize(InputPort port);

        /// <summary>
        /// Given index i, extract that item from the input collection.
        /// </summary>
        /// <param name="input">input port</param>
        /// <param name="i">item index</param>
        /// <returns></returns>
        protected abstract T Extract(InputPort input, int i);

        /// <summary>
        /// Given index i, place the item in the output collection.
        /// </summary>
        /// <param name="outputPort">output port</param>
        /// <param name="i">item index</param>
        /// <param name="value">item to put</param>
        protected abstract void Put(OutputPort outputPort, int i, U value);

        protected void ResetNodeStatusInLoop()
        {
            if (_nodesInLoop.Count == 0)
            {
                void AddRecursive(RhNode node)
                {
                    if (node is null) return;
                    if (node == this) return;
                    if (!node.flags.HasFlag(RhNodeFlag.PreventLoopResetPropagate))
                        _nodesInLoop.Add(node);
                    foreach (var inputPort in node.InputPortsGeneric)
                    {
                        foreach (var connectedPort in inputPort.GetConnectedPorts())
                        {
                            AddRecursive(connectedPort.node);
                        }
                    }
                }
                foreach (var port in itemResult.GetConnectedPorts())
                {
                    AddRecursive(port.node);
                }
            }
            foreach (var node in _nodesInLoop)
            {
                node.status = RhNodeStatus.Idle;
            }
        }

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            // todo: stub
            for (int i = 0; i < GetInputCollectionSize(collectionInput); i++)
            {
                indexPort.SetValueNoBoxing(i);
                ResetNodeStatusInLoop();
                itemPort.SetValueNoBoxing(Extract(collectionInput, i));
                foreach (var connectedPort in itemResult.GetConnectedPorts())
                {
                    connectedPort.node.EvaluateNode(context);
                    connectedPort.ForwardValue(itemResult);
                }
                Put(collectionOutput, i, itemResult.GetValueNoBoxing());
            }
            return true;
        }

        public override RhPort GetOutputPortByGUID(string guid)
        {
            if (itemPort.GUID == guid) return itemPort;
            if (indexPort.GUID == guid) return indexPort;
            return base.GetOutputPortByGUID(guid);
        }

        public override RhPort GetInputPortByGUID(string guid)
        {
            if (itemResult.GUID == guid) return itemResult;
            return base.GetInputPortByGUID(guid);
        }
    }
}