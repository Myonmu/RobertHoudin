using System;
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

        public T GetValue(Agent agent, Blackboard blackboard)
        {
            InitializeBindings(agent, blackboard);
            value = sourceType switch {
                SourceType.BlackBoard => getter.Invoke(),
                SourceType.Agent => getter.Invoke(),
                SourceType.VariableBoard => agent.variableBoard.GetValue<T>(sourceName),
                _ => value
            };
            return value;
        }

        public void SetValue(T val, Agent agent, Blackboard blackboard)
        {
            InitializeBindings(agent,blackboard);
            switch (sourceType) {
                case SourceType.BlackBoard:
                    setter.Invoke(val);
                    break;
                case SourceType.Agent:
                    setter.Invoke(val);
                    break;
                case SourceType.VariableBoard:
                    agent.variableBoard.SetValue(sourceName, val);
                    break;
                case SourceType.None:
                    value = val;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InitializeBindings(Agent agent, Blackboard blackboard)
        {
            switch (sourceType)
            {
                case SourceType.BlackBoard:
                    getter ??= ReflectionUtils.CreateGetter<T>(blackboard, sourceName);
                    setter ??= ReflectionUtils.CreateSetter<T>(blackboard, sourceName);
                    break;
                case SourceType.Agent:
                    getter ??= ReflectionUtils.CreateGetter<T>(agent, sourceName);
                    setter ??= ReflectionUtils.CreateSetter<T>(agent, sourceName);
                    break;
                case SourceType.VariableBoard:
                    break;
                case SourceType.None:
                    break;
            }
        }
    }
}