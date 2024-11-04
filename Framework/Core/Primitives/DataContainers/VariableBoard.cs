using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.MochiVariable;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.DataContainers
{
    public class VariableBoard : MonoBehaviour
    {
        [SerializeReference]
        public List<IMochiVariableBase> composites = new ();

        private Dictionary<string, IMochiVariableBase> dict = new();

        private void Start()
        {
            foreach (var mochiVariable in composites) {
                mochiVariable.InitializeBinding();
            }
            ConvertToDictionary();
        }

        public object GetValue(string key)
        {
            return dict.TryGetValue(key, out var val) ? val.BoxedValue : null;
        }

        public T GetValue<T>(string key)
        {
            if (dict.TryGetValue(key, out var val)) return (T)val.BoxedValue;
            throw new InvalidOperationException($"Variable of key = {key} can not be found.");
        }

        public void SetValue(string key, object value)
        {
            if (dict.TryGetValue(key, out var val)) val.BoxedValue = value;
        }

        public void SetValue<T>(string key, T value)
        {
            if (dict.TryGetValue(key, out var val)) val.BoxedValue = value;
        }

        public void ConvertToDictionary()
        {
            foreach (var variableBase in composites)
            {
                dict.TryAdd(variableBase.Key,variableBase);
            }
        }
    }
}