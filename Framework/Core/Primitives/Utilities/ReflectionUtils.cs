using System;
using System.Linq.Expressions;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.DataContainers;

namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    /// <summary>
    /// Dark magic. Not friend of IL2CPP (Source generator required if it is the case)
    /// </summary>
    internal static class ReflectionUtils
    {
        public static Func<T> CreateGetter<T>(object selectedComponent, FieldInfo field)
        {
            try
            {
                var expr = Expression.Field(field.IsStatic ? null : Expression.Constant(selectedComponent), field);
                return Expression.Lambda<Func<T>>(expr).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Action<T> CreateSetter<T>(object selectedComponent, FieldInfo field)
        {
            try
            {
                var param = Expression.Parameter(typeof(T));
                var expr = Expression.Field(field.IsStatic ? null : Expression.Constant(selectedComponent), field);
                var assign = Expression.Assign(expr, param);
                return Expression.Lambda<Action<T>>(assign, param).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Func<T> CreateNestedGetter<T>(object selectedComponent, PropertyInfo property,
            PropertyInfo second)
        {
            try
            {
                var expr = Expression.Property(Expression.Constant(selectedComponent), property);
                expr = Expression.Property(expr, second);
                return Expression.Lambda<Func<T>>(expr).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Action<T> CreateNestedSetter<T>(object selectedComponent, PropertyInfo property,
            PropertyInfo second)
        {
            try
            {
                var param = Expression.Parameter(typeof(T));
                var expr = Expression.Property(Expression.Constant(selectedComponent), property);
                expr = Expression.Property(expr, second);
                var assign = Expression.Assign(expr, param);
                return Expression.Lambda<Action<T>>(assign, param).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Func<T> CreateNestedGetter<T>(object selectedComponent, FieldInfo field, PropertyInfo second)
        {
            try
            {
                var expr = Expression.Field(field.IsStatic ? null : Expression.Constant(selectedComponent), field);
                expr = Expression.Property(expr, second);
                return Expression.Lambda<Func<T>>(expr).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Action<T> CreateNestedSetter<T>(object selectedComponent, FieldInfo field, PropertyInfo second)
        {
            try
            {
                var param = Expression.Parameter(typeof(T));
                var expr = Expression.Field(field.IsStatic ? null : Expression.Constant(selectedComponent), field);
                expr = Expression.Property(expr, second);
                var assign = Expression.Assign(expr, param);
                return Expression.Lambda<Action<T>>(assign, param).Compile();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Func<T> CreateNestedGetter<T>(object instance, FieldInfo first, FieldInfo second)
        {
            try
            {
                var expr = Expression.Field(Expression.Constant(instance), first);
                expr = Expression.Field(expr, second);
                return Expression.Lambda<Func<T>>(expr).Compile();
            }
            catch
            {
                return null;
            }
        }

        public static Action<T> CreateNestedSetter<T>(object instance, FieldInfo first, FieldInfo second)
        {
            try
            {
                var arg = Expression.Parameter(typeof(T));
                var expr = Expression.Field(Expression.Constant(instance), first);
                expr = Expression.Field(expr, second);
                var assignment = Expression.Assign(expr, arg);
                return Expression.Lambda<Action<T>>(assignment, arg).Compile();
            }
            catch
            {
                return null;
            }
        }

        public static Func<T> CreateNestedGetter<T>(object instance, PropertyInfo first, FieldInfo second)
        {
            try
            {
                var expr = Expression.Property(Expression.Constant(instance), first);
                expr = Expression.Field(expr, second);
                return Expression.Lambda<Func<T>>(expr).Compile();
            }
            catch
            {
                return null;
            }
        }

        public static Action<T> CreateNestedSetter<T>(object instance, PropertyInfo first, FieldInfo second)
        {
            try
            {
                var arg = Expression.Parameter(typeof(T));
                var expr = Expression.Property(Expression.Constant(instance), first);
                expr = Expression.Field(expr, second);
                var assignment = Expression.Assign(expr, arg);
                return Expression.Lambda<Action<T>>(assignment, arg).Compile();
            }
            catch
            {
                return null;
            }
        }

        public static T GetFieldValue<T>(object o, string fieldName)
        {
            var field = o.GetType().GetField(fieldName);
            return (T)field.GetValue(o);
        }

        public static void SetFieldValue<T>(object o, string fieldName, T value)
        {
            var field = o.GetType().GetField(fieldName);
            field.SetValue(o, value);
        }

        //Same as the first method but reduces 1 boxing-unboxing
        public static T GetValueFromBlackboard<T>(Blackboard b, string fieldName)
        {
            return (T)b.GetType().GetField(fieldName).GetValue(b);
        }

        public static T GetValueFromAgent<T>(RhPropertyBlock a, string fieldName)
        {
            return (T)a.GetType().GetField(fieldName).GetValue(a);
        }

        public static Action<T> CreateSetter<T>(object container, string fieldName)
        {
            var field = container.GetType().GetField(fieldName);
            if (field is not null) return CreateSetter<T>(container, field);
            var prop = container.GetType().GetProperty(fieldName);
            return (Action<T>)prop?.GetGetMethod().CreateDelegate(typeof(Action<T>), container);
        }

        public static Func<T> CreateGetter<T>(object container, string fieldName)
        {
            var field = container.GetType().GetField(fieldName);
            if (field is not null) return CreateGetter<T>(container, field);
            var prop = container.GetType().GetProperty(fieldName);
            return (Func<T>)prop?.GetGetMethod().CreateDelegate(typeof(Action<T>), container);
        }

        public static Action<T> CreateSetter<TS, T>(TS container, string fieldName)
        {
            var field = container.GetType().GetField(fieldName);
            if (field is not null) return CreateSetter<T>(container, field);
            var prop = container.GetType().GetProperty(fieldName);
            return (Action<T>)prop?.GetGetMethod().CreateDelegate(typeof(Action<T>), container);
        }

        public static Func<T> CreateGetter<TS, T>(TS container, string fieldName)
        {
            var field = container.GetType().GetField(fieldName);
            if (field is not null) return CreateGetter<T>(container, field);
            var prop = container.GetType().GetProperty(fieldName);
            return (Func<T>)prop?.GetGetMethod().CreateDelegate(typeof(Action<T>), container);
        }
    }
}