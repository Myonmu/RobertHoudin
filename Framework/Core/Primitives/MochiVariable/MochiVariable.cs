using System;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.MochiVariable
{
    public enum BindingMode
    {
        Value,
        GO,
        SO
    }
    [Serializable]
    public class MochiVariable<T>: IMochiVariableBase
    {
        public string key;
        [SerializeField] private BindingMode bindVariable = BindingMode.Value;
        [SerializeField] private T val;
        [SerializeField] private GoBindingSource<T> goBindingSource = new();
        [SerializeField] private SoBindingSource<T> soBindingSource = new();

        public void InitializeBinding()
        {
            //Delegates are not serialized, therefore each mochi var must be rebound at the start of the game.
            switch (bindVariable) {

                case BindingMode.Value:
                    break;
                case BindingMode.GO:
                    goBindingSource.Bind();
                    break;
                case BindingMode.SO:
                    soBindingSource.Bind();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string Key => key;

        object IMochiVariableBase.BoxedValue
        {
            get => Value;
            set => this.Value = (T)value;
        }

        public T Value {
            get {
                return bindVariable switch {
                    BindingMode.Value => val,
                    BindingMode.GO => goBindingSource.getValue.Invoke(),
                    BindingMode.SO => soBindingSource.getValue.Invoke(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set {
                switch (bindVariable) {
                    case BindingMode.Value:
                        val = value;
                        break;
                    case BindingMode.GO:
                        goBindingSource.setValue.Invoke(value);
                        break;
                    case BindingMode.SO:
                        soBindingSource.setValue.Invoke(value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

    }
}