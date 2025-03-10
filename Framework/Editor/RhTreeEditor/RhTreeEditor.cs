using System;
using RobertHoudin.Framework.Core.Primitives;
using RobertHoudin.Framework.Core.Primitives.DataContainers;
using RobertHoudin.Framework.Editor.Data;
using RobertHoudin.Framework.Editor.Misc;
using RobertHoudin.Framework.Editor.Node;
using RobertHoudin.Framework.Editor.Settings;
using RobertHoudin.Framework.Editor.Tree;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
namespace RobertHoudin.Framework.Editor.RhTreeEditor
{
    public class RhTreeEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset visualTree;
        private IRhPropertyBlock agent;
        private InspectorView blackboardView;
        //private SerializedProperty blackboardProperty;

        //private IMGUIContainer blackboardView;
        private InspectorView inspectorView;
        private RhTreeRunner runner;
        private RhTreeSettings settings;

        //private SerializedObject treeObject;
        private RhTreeView treeView;
        private VariableBoard variableBoard;
        
        public RhTree Tree => treeView?.tree;

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            DataSourceDrawer.OnDataSourceTypeChanged -= OnDataSourceTypeChanged;
            DataSourceDrawer.OnDataSourceTypeChanged += OnDataSourceTypeChanged;
        }


        private void OnDisable()
        {
            DataSourceDrawer.OnDataSourceTypeChanged -= OnDataSourceTypeChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnDataSourceTypeChanged()
        {
            treeView?.UpdateCullingStates();
        }

        public void CreateGUI()
        {
            settings = RhTreeSettings.GetOrCreateSettings();
            if (settings.behaviorTreeXml is null || settings.behaviorTreeStyle is null) return;
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Instantiate UXML
            visualTree = settings.behaviorTreeXml;
            visualTree.CloneTree(root);

            var styleSheet = settings.behaviorTreeStyle;
            root.styleSheets.Add(styleSheet);

            treeView = root.Q<RhTreeView>();
            treeView.AddSearchWindow(this);
            inspectorView = root.Q<InspectorView>();
            blackboardView = root.Q<InspectorView>("Blackboard");
            // blackboardView = root.Q<IMGUIContainer>();
            // blackboardView.onGUIHandler = () => {
            //     if (treeObject?.targetObject is null) return;
            //     treeObject?.Update();
            //     EditorGUILayout.PropertyField(blackboardProperty,includeChildren:true);
            //     treeObject?.ApplyModifiedProperties();
            // };
            treeView.onNodeSelected = OnNodeSelectionChanged;
            treeView.onSetOutputFlag = OnSetOutputFlag;
            //root.Q<Button>("blackboardButton").clicked += () => blackboardView?.UpdateBlackBoard(treeView.tree);
            root.Q<Button>("variableButton").clicked +=
                () => blackboardView?.UpdateVariableBoard(variableBoard);
            root.Q<Button>("agentButton").clicked +=
                () => blackboardView?.UpdateAgent(agent);
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeObject is null) return;
            var tree = Selection.activeObject as RhTree;
            if (Application.isPlaying) {
                if (tree)
                    treeView?.PopulateView(tree);
            } else {
                if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                    treeView?.PopulateView(tree);
            }

            //if (tree != null) blackboardView?.UpdateBlackBoard(tree);
            //     treeObject = new SerializedObject(tree);
            //     blackboardProperty = treeObject.FindProperty("blackboard");



        }

        [MenuItem("Tools/Robert Houdin Editor")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<RhTreeEditor>();
            wnd.titleContent = new GUIContent("Robert Houdin");
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj) {

                case PlayModeStateChange.EnteredEditMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    OnSelectionChange();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        private void OnNodeSelectionChanged(RhNodeView node)
        {
            inspectorView.UpdateSelection(node);
        }

        private void OnSetOutputFlag(RhNodeView node)
        {
            node.OnBecomeOutputNode();
            var prev = treeView.tree.SetResultNode(node.node);
            treeView.UpdateCullingStates();
            if (prev == node.node) return;
            if (prev == null) return;
            (treeView.FindNode(prev) as RhNodeView)?.OnBecomeNonOutputNode();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId)
        {
            if (Selection.activeObject is not RhTree tree) return false;
            OpenWindow();
            return true;
        }
    }
}