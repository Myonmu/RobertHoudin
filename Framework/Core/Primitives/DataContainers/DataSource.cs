using System;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Ports;
using RobertHoudin.Framework.Core.Primitives.Utilities;

namespace RobertHoudin.Framework.Core.Primitives.DataContainers
{
    /* Note on source type:
     * Port and None are the same here, but they differ on how they are evaluated.
     * 
     * When source type is set to None, the port becomes inactive and will not propagate
     * node evaluation, therefore fetching the default value.
     * 
     * When source type is set to Port, the port becomes active and the value of the node
     * will be set by evaluation process.
     *
     * See RhPort.IsActive
     */
    
    [Serializable]
    public class DataSource<T>
    {
        public SourceType sourceType = SourceType.Port;
        public string sourceName;

        /// <summary>
        /// Value of the data source. Must Bind the data source before accessing this field
        /// </summary>
        public T value;

        private Action<T> _setter;
        private Func<T> _getter;

        public T GetValue(RhExecutionContext context, RhNode node)
        {
            InitializeBindings(context, node);
            return GetValue();
        }

        public T GetValue()
        {
            value = sourceType switch
            {
                SourceType.PropertyBlock => _getter.Invoke(),
                _ => value
            };
            return value;
        }

        public void SetValue(T val, RhExecutionContext context, RhNode node)
        {
            InitializeBindings(context, node);
            switch (sourceType)
            {
                case SourceType.PropertyBlock:
                    _setter.Invoke(val);
                    break;
                case SourceType.None:
                    value = val;
                    break;
                case SourceType.Port:
                    value = val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitializeBindings(RhExecutionContext context, RhNode node)
        {
            switch (sourceType)
            {
                case SourceType.PropertyBlock:
                    _getter ??= ReflectionUtils.CreateGetter<T>(context.propertyBlock, sourceName);
                    _setter ??= ReflectionUtils.CreateSetter<T>(context.propertyBlock, sourceName);
                    break;
                case SourceType.Port:
                case SourceType.None:
                    break;
            }
        }
    }
}