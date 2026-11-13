using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public partial class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;

        public void UpdateSelectView(NodeView nodeView)
        {
            Clear();
            
            // 提前销毁旧的Editor,防止内存泄露
            UnityEngine.Object.DestroyImmediate(editor);

            if (nodeView == null || nodeView.nodeData == null) return;

            editor = UnityEditor.Editor.CreateEditor(nodeView.nodeData);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor != null && editor.target != null)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}