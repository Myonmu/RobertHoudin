using Plugins.RobertHoudin.Utils;
using RobertHoudin.Splines.Runtime;
using RobertHoudin.Splines.Runtime.CustomData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
namespace Plugins.RobertHoudin.Splines.Editor
{
    [CustomPropertyDrawer(typeof(RhSplineCustomData<>),true)]
    public class RhSplineCustomDataDrawer : PropertyDrawer
    {
        private VisualTreeAsset _asset;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            _asset ??= AssetDbShorthand.FindAndLoadFirst<VisualTreeAsset>(nameof(RhSplineCustomDataDrawer));
            var tree = _asset.CloneTree();
            var data = (property.boxedValue as RhSplineCustomData);
            tree.Q<TextField>().BindProperty(property.FindPropertyRelative("dataKey"));
            tree.Q<DropdownField>().RegisterValueChangedCallback(
                (evt => data.dataKey = evt.newValue)
            );
            var candidateProp = property.FindPropertyRelative("candidateData");
            var cpid = tree.Q<IntegerField>("ControlPointId");
            cpid.BindProperty(candidateProp.FindPropertyRelative("controlPointIndex"));
            var candidateData = tree.Q<PropertyField>("CandidateData");
            candidateData.BindProperty(candidateProp.FindPropertyRelative("data"));
            var container = tree.Q<VisualElement>("CandidateContainer");
            
            
            
            var allData = tree.Q<MultiColumnListView>("AllData");
            allData.showBoundCollectionSize = false;
            allData.columns.Add(new Column()
            {
                title = "control point id",
                width = 100,
                bindingPath = "controlPointIndex"
            });
            allData.columns.Add(new Column()
            {
                title = "value",
                width = 300,
                bindingPath = "data"
            });
            allData.columns.Add(new Column()
            {
                title = "del",
                width = 100,
                bindingPath = "controlPointIndex",
                makeCell = (() => new Button()),
                bindCell = ((element, i) => {
                    var button = element.Q<Button>();
                    button.clicked += () => {
                        data.Remove(data.GetNthDataId(i));
                    };
                } )
            });
            allData.BindProperty(property.FindPropertyRelative("entries"));
            
            tree.Q<Button>().clicked += () => {
                data.PushCandidate();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            };
            
            
            return tree;
        }
    }
}