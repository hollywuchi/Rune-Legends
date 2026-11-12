using System;
using BehaviorTree.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        private BehaviorTreeView _treeView;
        
        [MenuItem("Tools/BehaviorTreeEditor")] 
        private static void ShowWindow()
        {
            var window = GetWindow<BehaviorTreeEditorWindow>();
            window.titleContent = new GUIContent("Behavior Tree Editor");
            window.Show();
        }

        private void CreateGUI()
        {
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            splitView.style.flexGrow = 1.0f;
            rootVisualElement.Add(splitView);

            var inspectorView = new InspectorView();
            var rightView = new VisualElement();
            rightView.style.flexGrow = 1.0f;
            
            _treeView = new BehaviorTreeView();
            _treeView.style.flexGrow = 1.0f;
            
            rightView.Add(_treeView);
            splitView.Add(inspectorView);
            splitView.Add(rightView);

            _treeView.OnNodeSelected = inspectorView.UpdateSelectView; 
            

            var sheetStyle =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Editor/BehaviorTreeEditor/BehaviorTreeEditor.uss");

            if (sheetStyle != null)
                _treeView.styleSheets.Add(sheetStyle);
            OnSelectionChange();
        }
        
        /// <summary>
        /// 原生生命周期，只要在编辑器中点击了不同的东西，就会触发
        /// </summary>
        private void OnSelectionChange()
        {
            BehaviorTreeSO tree = Selection.activeObject as BehaviorTreeSO;

            if (tree == null && Selection.activeGameObject != null)
            {
                var runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if (runner != null)
                    // 当游戏运行时，点击怪物就能直接看到他身上的树
                    tree = Application.isPlaying ? runner._runtimeTree : runner.treeAssets;
            }

            if (tree != null && _treeView != null)
            {
                _treeView.PopulateView(tree);
            }
        }

        public void OnInspectorUpdate()
        {
            _treeView?.UpdateNodeStatus();
        }
    }
}