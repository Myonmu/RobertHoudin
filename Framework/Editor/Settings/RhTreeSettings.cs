using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//Adjusted version based on The Kiwi Coder's original version. 
namespace MochiBTS.Editor
{
    internal class RhTreeSettings : ScriptableObject
    {
        public VisualTreeAsset behaviorTreeXml;
        public StyleSheet behaviorTreeStyle;
        public VisualTreeAsset nodeXml;
        public StyleSheet nodeStyle;
        public List<Texture2D> nodeIcons = new();
        private static RhTreeSettings FindSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{nameof(RhTreeSettings)}");
            if (guids.Length > 1)
                Debug.LogWarning("Found multiple settings files, using the first.");

            switch (guids.Length) {
                case 0:
                    Debug.LogWarning("Settings not found...");
                    return null;
                default:
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath<RhTreeSettings>(path);
            }
        }

        internal static RhTreeSettings GetOrCreateSettings()
        {
            var settings = FindSettings();
            if (settings != null)
                return settings;
            settings = CreateInstance<RhTreeSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/TATools/MochiBTS/RhSettings.asset");
            AssetDatabase.SaveAssets();
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
        
        internal Texture2D GetIcon(RhNode node)
        {
            var currentClass = node.GetType();
            while (currentClass is not null) {
                var result = nodeIcons.Find((x) => x.name == currentClass.Name);
                if (result is not null) return result;
                currentClass = currentClass.BaseType;
            }
            return null;
        }
    }

// Register a SettingsProvider using UIElements for the drawing framework:
    internal static class MyCustomSettingsUIElementsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("Project/MyCustomUIElementsSettings", SettingsScope.Project) {
                label = "Robert Houdin",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) => {
                    var settings = RhTreeSettings.GetSerializedSettings();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    var title = new Label {
                        text = "Rh Tree Settings"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement {
                        style = {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new InspectorElement(settings));

                    rootElement.Bind(settings);
                }
            };

            return provider;
        }
        
    }


}