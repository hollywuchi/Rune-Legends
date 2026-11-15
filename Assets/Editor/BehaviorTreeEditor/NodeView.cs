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

        private Label description;

        public NodeView(BtBaseNode _nodeData)
        {
            nodeData = _nodeData;
            title = _nodeData.name;
            viewDataKey = _nodeData.Guid;

            SetPosition(new Rect(_nodeData.position, Vector2.zero));

            CreateDescriptionLabel();
            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateInputPorts()
        {
            if (nodeData is RootNode) return;
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

        private void CreateDescriptionLabel()
        {
            description = new Label
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal, // 支持自动换行
                    maxWidth = 180, // 限制宽度防止节点被长备注撑太宽
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 4,
                    paddingBottom = 4,
                    fontSize = 11,
                    color = new Color(0.8f, 0.8f, 0.8f, 0.9f),
                    backgroundColor = new Color(0f, 0f, 0f, 0.75f)
                }
            };
            int titleIndex = mainContainer.IndexOf(titleContainer);
            mainContainer.Insert(titleIndex + 1, description);
            
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            if (nodeData == null || description == null) return;

            var hasTxt = !string.IsNullOrWhiteSpace(nodeData.descrpition);
            description.text = nodeData.descrpition;
            description.style.display = hasTxt ? DisplayStyle.Flex : DisplayStyle.None;
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
        public void SortChildren(BtBaseNode nodeData)
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

            if (Time.unscaledTime - nodeData.LastEvaluateTime > 0.2f)
            {
                // 如果当前节点过了0.2s仍然没有更新，那么默认这个节点为“非活跃”，将其颜色刷新掉
                titleContainer.style.backgroundColor = StyleKeyword.Null;
                return;
            }

            switch (nodeData.State)
            {
                case BtLifeTimeState.Running:
                    // running节点同步刷新
                    titleContainer.style.backgroundColor = nodeData.Started
                        ? new StyleColor(new Color(0.15f, 0.35f, 0.55f))
                        : StyleKeyword.Null;
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