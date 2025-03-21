﻿using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Utilities;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public interface IDataSourcePort
    {
        public SourceType SourceType { get; }
        public string SourceName { get; }
        void EvalSource(RhExecutionContext ctx, RhNode node);
    }
    public abstract class RhDataSourcePort<T> : RhPort<T>, IDataSourcePort
    {
        public DataSource<T> value = new(){sourceType = SourceType.Port};
        public override bool IsImplicitlyDisabled => value.sourceType != SourceType.Port;
        public SourceType SourceType => value.sourceType;
        public string SourceName => value.sourceName;
        public void EvalSource(RhExecutionContext ctx, RhNode node)
        {
            value.InitializeBindings(ctx, node);
        }
        public override T GetValueNoBoxing()
        {
            return value.GetValue();
        }

        public override void SetValueNoBoxing(T value)
        {
            this.value.value = value;
        }
    }

    public static class RhDataSourcePortUtils
    {
        public static void EvalDataSources(this RhNode node, RhExecutionContext ctx)
        {
            foreach (var p in node.GetPortsImplementing<IDataSourcePort>())
            {
                (p as IDataSourcePort).EvalSource(ctx, node);
            }
        }
    }
}