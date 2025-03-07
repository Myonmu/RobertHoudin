using System;
namespace RobertHoudin.SGPatcher
{
    public static class SGPropertyTypeMapping
    {
        private const string FQDNTemplate = "UnityEditor.ShaderGraph.Internal.{0}ShaderProperty";
        public static string GetFQDN(this SGPropertyType propertyType)
        {
            var name = propertyType switch
            {
                SGPropertyType.Float => "Vector1",
                _ => Enum.GetName(typeof(SGPropertyType), propertyType)
            };
            return string.Format(FQDNTemplate, name);
        }
    }
}