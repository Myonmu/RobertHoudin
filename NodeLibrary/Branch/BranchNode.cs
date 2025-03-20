using System;
using RobertHoudin.Framework.Core.Ports;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
namespace RobertHoudin.NodeLibrary.Branch
{
    public abstract class BranchNode<T, TInputPort, TOutputPort> : RhNode
    where TInputPort: RhPort<T>
    where TOutputPort: RhPort<T>
    {
        [RhInputPort] public BoolPortDs condition;
        [RhInputPort] public TInputPort a;
        [RhInputPort] public TInputPort b;
        [RhOutputPort] public TOutputPort output;
        public override void EvaluateNode(RhExecutionContext context, EvalMode mode = EvalMode.Normal)
        {
            if (mode is EvalMode.Normal && status is not RhNodeStatus.Idle)
            {
                return;
            }

            OnBeginEvaluate(context);
            status = RhNodeStatus.WaitingForDependency;

            // eval condition first. (for each but really there's only one connected port)
            foreach (var port in condition.GetConnectedPorts())
            {
                port.node.EvaluateNode(context, mode);
                port.ForwardValue(condition);
                if (port.node.status is RhNodeStatus.Failed) throw new Exception("Failed to evaluate condition");
            }
            
            // eval branch based on condition eval result
            if (condition.GetValueNoBoxing())
            {
                foreach (var port in a.GetConnectedPorts())
                {
                    port.node.EvaluateNode(context, mode);
                    port.ForwardValue(a);
                    if (port.node.status is RhNodeStatus.Failed) throw new Exception("Failed to evaluate A (true) branch");
                } 
            }
            else
            {
                foreach (var port in b.GetConnectedPorts())
                {
                    port.node.EvaluateNode(context, mode);
                    port.ForwardValue(b);
                    if (port.node.status is RhNodeStatus.Failed) throw new Exception("Failed to evaluate B (false) branch");
                } 
            }

            status = OnEvaluate(context) ? RhNodeStatus.Success : RhNodeStatus.Failed;
        }

        protected override bool OnEvaluate(RhExecutionContext context)
        {
            output.SetValueNoBoxing(condition.GetValueNoBoxing() ? a.GetValueNoBoxing() : b.GetValueNoBoxing());
            return true;
        }
    }
}