using BehaviorTree.Core;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class NodeView : Node
    {
        public BtBaseNode nodeData;
        public Port input;
        public Port output;
        public System.Action<NodeView> OnNodeSelected;

        public NodeView(BtBaseNode _nodeData)
        {
            nodeData = _nodeData;
            title = _nodeData.name;
            viewDataKey = _nodeData.Guid;
            
            SetPosition(new Rect(_nodeData.position,Vector2.zero));
            
            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateInputPorts()
        {
            if(nodeData is RootNode) return;
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            if (input != null)
            {
                input.portName = "In";
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts()
        {
            if (nodeData is CompositeNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            else if (nodeData is DecoratorNode or RootNode)
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            if (output == null) return;
            output.portName = "Out";
            outputContainer.Add(output);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            nodeData.position.x = newPos.x;
            nodeData.position.y = newPos.y;
            
            if (input is { connected: true })
            {
                foreach (var items in input.connections)
                {
                    if (items.output.node is NodeView nodeView)
                    {
                        nodeView.SortChildren(nodeView.nodeData);
                    }
                }
            }
            
#if UNITY_EDITOR
            // 标记资产已被修改，触发保存
            EditorUtility.SetDirty(nodeData);
#endif
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
        
        /// <summary>
        /// 为编辑器中的选项排序，从左到右依次执行
        /// </summary>
        /// <param name="nodeData"></param>
        private void SortChildren(BtBaseNode nodeData)
        {
            if (nodeData == null) return;
            if (nodeData is CompositeNode compositeNode)
            {
                // 按X坐标从小到大排列
                compositeNode.children.Sort((left, right) 
                    => left.position.x < right.position.x ? -1 : 1);
            }
        }


        public void UpdateState()
        {
            if (!Application.isPlaying || nodeData == null) return;
            switch (nodeData.State)
            {
                case BtLifeTimeState.Running:
                    if (nodeData.Started)
                        titleContainer.style.backgroundColor = new StyleColor(new Color(0.15f, 0.35f, 0.55f));
                    break;
                case BtLifeTimeState.Success:
                    titleContainer.style.backgroundColor = new StyleColor(new Color(0.18f, 0.55f, 0.34f));
                    break;
                case BtLifeTimeState.Failure:
                    titleContainer.style.backgroundColor = new StyleColor(new Color(0.55f, 0.2f, 0.2f));
                    break;
            }
        }
    }
}