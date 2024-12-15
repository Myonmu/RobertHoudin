using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RobertHoudin.Framework.Core.Primitives.Nodes;
namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public static class PortReflection
    {
        #region port reflection
        public static List<RhPort> GetPortsMatchingPredicate(this RhNode node, Predicate<FieldInfo> predicate)
        {
            return node.GetType().GetFields(BindingFlags.Public
                                       | BindingFlags.Instance
                                       | BindingFlags.FlattenHierarchy)
                .Where(x => predicate(x))
                .Select(x =>
                {
                    var p = x.GetValue(node) as RhPort;
                    p.name = x.Name;
                    return p;
                }).ToList();
        }

        public static List<RhPort> GetPortsWithAttribute<T>(this RhNode node) where T : Attribute
        {
            return node.GetPortsMatchingPredicate(x => x.GetCustomAttribute<T>() is not null);
        }

        public static List<RhPort> GetPortsDerivedFrom<T>(this RhNode node) where T : RhPort
        {
            return node.GetPortsMatchingPredicate(x => x.FieldType.IsSubclassOf(typeof(T)));
        }

        public static List<RhPort> GetPortsImplementing<T>(this RhNode node)
        {
            return node.GetPortsMatchingPredicate(x => x.FieldType.GetInterfaces().Contains(typeof(T)));
        }
        
        #endregion
    }
}