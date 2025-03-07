using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.DataContainers;

namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    /// <summary>
    /// Dark magic. Not friend of IL2CPP (Source generator required if it is the case)
    /// </summary>
    public static class ReflectionUtils
    {
        public static Func<T> CreateGetter<T>(object selectedComponent, FieldInfo field)
        {
            try
            {
                var expr = Expression.Field(field.IsStatic ? null : Expression.Constant(selectedComponent), field);
                var convertedExpr = Expression.Convert(expr, typeof(T));
                return Expression.Lambda<Func<T>>(convertedExpr).Compile();
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
                return Expression.Lambda<Func<T>>(Expression.Convert(expr, typeof(T))).Compile();
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
                return Expression.Lambda<Func<T>>(Expression.Convert(expr, typeof(T))).Compile();
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
                return Expression.Lambda<Func<T>>(Expression.Convert(expr, typeof(T))).Compile();
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
                return Expression.Lambda<Func<T>>(Expression.Convert(expr, typeof(T))).Compile();
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

        public static T GetValueFromAgent<T>(IRhPropertyBlock a, string fieldName)
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

        public static List<T> GetFieldsImplementing<T>(object container)
        {
            return container.GetType().GetFields()
                .Where(x => x.FieldType.GetInterfaces().Contains(typeof(T)))
                .Select(x => (T)x.GetValue(container)).ToList();
        }

        public static T GetFirstFieldImplementing<T>(object container)
        {
            var f = container.GetType().GetFields()
                .First(x => x.FieldType.GetInterfaces().Contains(typeof(T)));
            return (T)f.GetValue(container);
        }

        public static List<FieldInfo> GetFieldsWithAttribute<T>(object container) where T : Attribute
        {
            return container.GetType().GetFields().Where(x => x.GetCustomAttribute<T>() != null).ToList();
        }

        public static List<FieldInfo> GetFieldsAssignableTo(Type type, Type containerType)
        {
            return containerType.GetFields(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => type.IsConvertibleTo(x.FieldType)).ToList();
        }
        public static IEnumerable<T> GetAllMembers<T>(this Type type, BindingFlags flags = BindingFlags.Default) where T : MemberInfo
        {
            if (type == (Type) null)
                throw new ArgumentNullException(nameof (type));
            if (type == typeof(object))
                yield break;
            var currentType = type;
            MemberInfo[] memberInfoArray;
            int index;
            if ((flags & BindingFlags.DeclaredOnly) == BindingFlags.DeclaredOnly)
            {
                memberInfoArray = currentType.GetMembers(flags);
                for (index = 0; index < memberInfoArray.Length; ++index)
                {
                    var allMember = memberInfoArray[index] as T;
                    if (allMember != null)
                        yield return allMember;
                }
            }
            else
            {
                flags |= BindingFlags.DeclaredOnly;
                do
                {
                    memberInfoArray = currentType.GetMembers(flags);
                    for (index = 0; index < memberInfoArray.Length; ++index)
                    {
                        var allMember = memberInfoArray[index] as T;
                        if (allMember != null)
                            yield return allMember;
                    }
                    currentType = currentType.BaseType;
                }
                while (currentType != null);
            }
        }
        public static MethodInfo GetCastMethod(this Type from, Type to, bool requireImplicitCast = false)
        {
            foreach (var allMember in from.GetAllMembers<MethodInfo>(BindingFlags.Static | BindingFlags.Public))
            {
                if ((allMember.Name == "op_Implicit" || !requireImplicitCast && allMember.Name == "op_Explicit") && allMember.GetParameters()[0].ParameterType.IsAssignableFrom(from) && to.IsAssignableFrom(allMember.ReturnType))
                    return allMember;
            }
            foreach (var allMember in to.GetAllMembers<MethodInfo>(BindingFlags.Static | BindingFlags.Public))
            {
                if ((allMember.Name == "op_Implicit" || !requireImplicitCast && allMember.Name == "op_Explicit") && allMember.GetParameters()[0].ParameterType.IsAssignableFrom(from) && to.IsAssignableFrom(allMember.ReturnType))
                    return allMember;
            }
            return (MethodInfo) null;
        }
        
        public static bool IsConvertibleTo(this Type from, Type to)
        {
            return to.IsAssignableFrom(from) || IsConvertibleToViaCastOrTypeConversion(from, to);
            bool IsConvertibleToViaCastOrTypeConversion(Type from, Type to)
            {
                if (from.IsEnum)
                    return Enum.GetUnderlyingType(from).IsConvertibleTo(to);
                if (to.IsEnum)
                    return Enum.GetUnderlyingType(to).IsConvertibleTo(from);
                if (!from.IsPrimitive || !to.IsPrimitive)
                    return from.GetCastMethod(to) != null;
                var tc = TypeDescriptor.GetConverter(from);
                return tc.CanConvertTo(to);
            }
        }
    }
}