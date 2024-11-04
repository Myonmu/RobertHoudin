using System;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RobertHoudin.Framework.Core.Primitives.MochiVariable
{
    [Serializable]
    public class GoBindingSource<T> : BindingSource
    {

        public GameObject obj;
        public Component selectedComponent;

        public Func<T> getValue;
        public Action<T> setValue;

        public override Object UnityObj {
            get => selectedComponent;
            set => selectedComponent = value as Component;
        }

        public override Object BaseObj { get => obj; set => obj = value as GameObject; }


        public object BoxedValue()
        {
            try {
                return getValue is null ? null : getValue.Invoke();
            } catch {
                ResetDelegate();
            }

            return null;
        }

        public void ResetDelegate()
        {
            getValue = null;
            setValue = null;
        }

        public Type GetValueType()
        {
            return typeof(T);
        }

        public void Bind()
        {
            if (selectedComponent is null || selectedProperty is null) return;
            var property1 = selectedComponent.GetType().GetProperty(selectedProperty);
            var field1 = selectedComponent.GetType().GetField(selectedProperty);
            if (property1 != null) {
                BindProp(property1, selectedSub);
            } else if (field1 != null) {
                BindField(field1, selectedSub);
            }
        }

        private void BindProp(PropertyInfo propertyInfo, string sub)
        {
            var type1 = propertyInfo.PropertyType;
            var property2 = type1.GetProperty(sub);
            var field2 = type1.GetField(sub);
            if (property2 is not null) {
                getValue = ReflectionUtils.CreateNestedGetter<T>(selectedComponent, propertyInfo, property2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(selectedComponent, propertyInfo, property2);
            } else if (field2 is not null) {
                getValue = ReflectionUtils.CreateNestedGetter<T>(selectedComponent, propertyInfo, field2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(selectedComponent, propertyInfo, field2);
            } else {
                getValue = (Func<T>)propertyInfo.GetGetMethod().CreateDelegate(typeof(Func<T>), selectedComponent);
                setValue = (Action<T>)propertyInfo.GetSetMethod().CreateDelegate(typeof(Action<T>), selectedComponent);
            }
        }

        private void BindField(FieldInfo fieldInfo, string sub)
        {
            var type1 = fieldInfo.FieldType;
            var property2 = type1.GetProperty(sub);
            var field2 = type1.GetField(sub);
            if (property2 is not null) {
                getValue = ReflectionUtils.CreateNestedGetter<T>(selectedComponent, fieldInfo, property2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(selectedComponent, fieldInfo, property2);
            } else if (field2 is not null) {
                getValue = ReflectionUtils.CreateNestedGetter<T>(selectedComponent, fieldInfo, field2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(selectedComponent, fieldInfo, field2);
            } else {
                getValue = ReflectionUtils.CreateGetter<T>(selectedComponent, fieldInfo);
                setValue = ReflectionUtils.CreateSetter<T>(selectedComponent, fieldInfo);
            }
        }
    }
}