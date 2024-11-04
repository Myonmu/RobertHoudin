using System;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RobertHoudin.Framework.Core.Primitives.MochiVariable
{
    [Serializable]
    public class SoBindingSource<T> : BindingSource
    {
        public ScriptableObject obj;

        public Func<T> getValue;
        public Action<T> setValue;

        public override Object UnityObj
        {
            get => obj;
            set => obj = value as ScriptableObject;
        }

        public object BoxedValue()
        {
            try
            {
                return getValue is null ? null : getValue.Invoke();
            }
            catch
            {
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
            if (obj is null || selectedProperty is null) return;
            var property1 = obj.GetType().GetProperty(selectedProperty);
            var field1 = obj.GetType().GetField(selectedProperty);
            if (property1 is not null)
            {
                BindProp(property1, selectedSub);
            }
            else if (field1 is not null)
            {
                BindField(field1, selectedSub);
            }
        }

        private void BindProp(PropertyInfo propertyInfo, string sub)
        {
            var type1 = propertyInfo.PropertyType;
            var property2 = type1.GetProperty(sub);
            var field2 = type1.GetField(sub);
            if (property2 is not null)
            {
                getValue = ReflectionUtils.CreateNestedGetter<T>(obj, propertyInfo, property2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(obj, propertyInfo, property2);
            }
            else if (field2 is not null)
            {
                getValue = ReflectionUtils.CreateNestedGetter<T>(obj, propertyInfo, field2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(obj, propertyInfo, field2);
            }
            else
            {
                getValue = (Func<T>)propertyInfo.GetGetMethod().CreateDelegate(typeof(Func<T>), obj);
                setValue = (Action<T>)propertyInfo.GetSetMethod().CreateDelegate(typeof(Action<T>), obj);
            }
        }

        private void BindField(FieldInfo fieldInfo, string sub)
        {
            var type1 = fieldInfo.FieldType;
            var property2 = type1.GetProperty(sub);
            var field2 = type1.GetField(sub);
            if (property2 is not null)
            {
                getValue = ReflectionUtils.CreateNestedGetter<T>(obj, fieldInfo, property2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(obj, fieldInfo, property2);
            }
            else if (field2 is not null)
            {
                getValue = ReflectionUtils.CreateNestedGetter<T>(obj, fieldInfo, field2);
                setValue = ReflectionUtils.CreateNestedSetter<T>(obj, fieldInfo, field2);
            }
            else
            {
                getValue = ReflectionUtils.CreateGetter<T>(obj, fieldInfo);
                setValue = ReflectionUtils.CreateSetter<T>(obj, fieldInfo);
            }
        }
    }
}