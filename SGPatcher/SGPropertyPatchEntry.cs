using System;
namespace RobertHoudin.SGPatcher
{
    [Serializable]
    public class SGPropertyPatchEntry
    {
        public SGPropertyType type;
        public string name;
        public string shaderVarName;
        public SGPrecision precision;
        public SGPropertyScope scope;
        public bool hidden;

        public bool OverrideHLSLDeclaration => scope is not SGPropertyScope.PerMaterial;
        public int HlslDeclarationOverride
        {
            get {
                return scope switch
                {
                    SGPropertyScope.Global => 1,
                    SGPropertyScope.PerMaterial => 0,
                    SGPropertyScope.HybridPerInstance => 3,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        public bool IsHidden => scope is SGPropertyScope.Global | hidden;
        public bool GeneratePropertyBlock => scope is not SGPropertyScope.Global;

        public SGPropertyJsonBlock GenerateJsonBlock()
        {
            return new SGPropertyJsonBlock()
            {
                hlslDeclarationOverride = HlslDeclarationOverride,
                m_DefaultReferenceName = shaderVarName,
                m_Type = type.GetFQDN(),
                m_Name = name,
                m_OverrideReferenceName = shaderVarName,
                m_Precision = (int)precision,
                m_Hidden = IsHidden,
                m_GeneratePropertyBlock = GeneratePropertyBlock,
                overrideHLSLDeclaration = OverrideHLSLDeclaration,
                m_RefNameGeneratedByDisplayName = name
            };
        }
    }
}