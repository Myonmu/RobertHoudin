using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;
namespace Plugins.RobertHoudin.Utils
{
    public static class AssetDbShorthand
    {
        public static T FindAndLoadFirst<T>(string filter = "") where T : Object
        {
            var result = AssetDatabase.FindAssets($"t:{typeof(T).Name} {filter}");
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(result[0]));
        }

        public static Object FindAndLoadFirst(Type type, string filter = "")
        {
            var result = AssetDatabase.FindAssets($"t:{type.Name} {filter}");
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(result[0]), type);
        }
        public static List<T> FindAndLoadAll<T>(string filter = "") where T : Object
        {
            var result = AssetDatabase.FindAssets($"t:{typeof(T).Name} {filter}");
            return result.Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToList();
        }
    }
}