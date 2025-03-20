using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
namespace RobertHoudin.SGPatcher
{
    [UnityEngine.CreateAssetMenu(fileName = "SGPropertyPatchPreset",
        menuName = "RobertHoudin/SG Property Patch Preset", order = 0)]
    public class SGPropertyPatchPreset : ScriptableObject
    {
        public List<SGPropertyPatchEntry> entries = new();
        public Shader sgToPatch;
        public ScriptableObject subGraphToPatch;
        
        [ContextMenu("Patch Shader Graph")]
        public void PatchShader()
        {
            Assert.IsNotNull(sgToPatch, "You forgot to select a Shader Graph in sgToPatch.");
            var asset = AssetDatabase.GetAssetPath(sgToPatch);
            Assert.IsTrue(asset.EndsWith(".shadergraph"), "sgToPatch is not a Shader Graph, did you put a sub graph or hand written shader there?");
            Patch(sgToPatch);
        }
        [ContextMenu("Patch Sub Graph")]
        public void PatchSubGraph()
        {
            Assert.IsNotNull(subGraphToPatch, "You forgot to select a Sub Graph in sgToPatch.");
            var asset = AssetDatabase.GetAssetPath(subGraphToPatch);
            Assert.IsTrue(asset.EndsWith(".shadersubgraph"), "target sub graph object is not a sub graph");
            Patch(subGraphToPatch);
        }
        public void Patch(Object target)
        {
            var objectList = new List<SGObjectReference>();
            var path = Path.Combine(Application.dataPath, "../", AssetDatabase.GetAssetPath(target));
            var rawStr = File.ReadAllText(path);
            var sb = new StringBuilder(rawStr);
            // generate new properties
            foreach (var entry in entries)
            {
                var obj = entry.GenerateJsonBlock();
                var nameField = obj.NameFieldString;
                if (rawStr.Contains(nameField))
                {
                    //TODO: perhaps override the existing data
                    Debug.LogWarning($"SG already contains {nameField}, this entry will be skipped.");
                    continue;
                }
                obj.GenerateGUID();
                objectList.Add(new()
                {
                    m_Id = obj.m_ObjectId
                });
                sb.AppendLine();
                sb.AppendLine(JsonUtility.ToJson(obj, true));
            }
            var result = sb.ToString();
            var unmodified = sb.ToString();
            // append object references
            const string childObjectListTemplate = "\"m_ChildObjectList\": [{0}]";
            const string childObjectListPattern = @"""m_ChildObjectList"": \[\s*(.*?)\s*\]";
            const string propertyListTemplate = "\"m_Properties\": [{0}]";
            const string propertyListPattern = @"""m_Properties"": \[\s*(.*?)\s*\]";

            result = Regex.Replace(result, childObjectListPattern, match =>
            {
                //seek back, attempt to verif if the category group is anonymous
                var start = match.Index - 1;
                var seekCursor = start;
                var seekLimit = start - 30;
                while (unmodified[seekCursor] != '\"' && seekCursor > seekLimit)
                {
                    seekCursor--;
                }

                if (seekCursor <= seekLimit || unmodified[seekCursor - 1] != '\"') 
                {
                    return match.Value;
                }
                
                var sb2 = StringBuilder(match);
                return string.Format(childObjectListTemplate, sb2);
            }, RegexOptions.Singleline);

            result = Regex.Replace(result, propertyListPattern, match => {
                var sb2 = StringBuilder(match);
                return string.Format(propertyListTemplate, sb2);
            }, RegexOptions.Singleline);

            File.WriteAllText(path, result);
            StringBuilder StringBuilder(Match match)
            {
                var data = match.Groups[1].Value;
                var sb2 = new StringBuilder(data);
                var isFirstAppend = true;
                foreach (var reference in objectList)
                {
                    if (!isFirstAppend || data.EndsWith("}"))
                    {
                        sb2.Append(",\n");
                    }
                    isFirstAppend = false;
                    sb2.Append(JsonUtility.ToJson(reference, true));
                    sb2.Append("\n");
                }
                return sb2;
            }
        }
    }
}