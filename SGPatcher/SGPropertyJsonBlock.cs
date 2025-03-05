using System;
using UnityEditor;
namespace Plugins.RobertHoudin.SGPatcher
{
    [Serializable]
    public class SGGUIDJsonBlock
    {
        public string m_GuidSerialized;
    }
    
    [Serializable]
    public class SGPropertyJsonBlock
    {
        public int m_SGVersion = 1;
        public string m_Type;
        public string m_ObjectId;
        public SGGUIDJsonBlock m_Guid = new();
        public string m_Name;
        public int m_DefaultRefNameVersion = 1;
        public string m_RefNameGeneratedByDisplayName;
        public string m_DefaultReferenceName;
        public string m_OverrideReferenceName;
        public bool m_GeneratePropertyBlock;
        public bool m_UseCustomSlotLabel = false;
        public string m_CustomSlotLabel = "";
        public int m_DismissedVersion;
        public int m_Precision;
        public bool overrideHLSLDeclaration;
        public int hlslDeclarationOverride;
        public bool m_Hidden;

        public string NameFieldString => $"\"m_Name\": \"{m_Name}\"";
        public void GenerateGUID()
        {
            m_ObjectId = Guid.NewGuid().ToString("N");
            m_Guid.m_GuidSerialized = Guid.NewGuid().ToString();
        }
    }
}