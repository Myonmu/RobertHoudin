using System;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using RobertHoudin.Framework.Core.Primitives.Utilities;

namespace RobertHoudin.Framework.Core.Primitives.DataContainers
{
    [Serializable]
    public class DataSource<T>
    {
        public SourceType sourceType;
        public string sourceName;

        /// <summary>
        /// Value of the data source. Must Bind the data source before accessing this field
        /// </summary>
        public T value;

        private Action<T> setter;
        private Func<T> getter;

        public T GetValue(RhExecutionContext context, RhNode node)
        {
            InitializeBindings(context, node);
            value = sourceType switch
            {
                SourceType.PropertyBlock => getter.Invoke(),
                SourceType.Port => getter.Invoke(),
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
                    setter.Invoke(val);
                    break;
                case SourceType.None:
                    value = val;
                    break;
                case SourceType.Port:
                    setter.Invoke(val);
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
                    getter ??= ReflectionUtils.CreateGetter<T>(context.propertyBlock, sourceName);
                    setter ??= ReflectionUtils.CreateSetter<T>(context.propertyBlock, sourceName);
                    break;
                case SourceType.Port:
                    getter ??= () => (T)node.InputPorts[int.Parse(sourceName)].GetValue();
                    setter ??= (v) => node.InputPorts[int.Parse(sourceName)].SetValue(v);
                    break;
                case SourceType.None:
                    break;
            }
        }
    }
}