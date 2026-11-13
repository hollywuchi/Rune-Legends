using System.Collections.Generic;
using BehaviorTree.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class BehaviorTreeView : GraphView
    {
        private BehaviorTreeSO currentTree;
        public System.Action<NodeView> OnNodeSelected;

        public BehaviorTreeView()
        {
            this.AddManipulator(new ContentZoomer()); // 滚轮缩放
            this.AddManipulator(new ContentDragger()); // 右键或中键移动
            this.AddManipulator(new SelectionDragger()); // 选中拖动
            this.AddManipulator(new RectangleSelector()); // 矩形选框

            GridBackground background = new GridBackground();
            Insert(0, background); // 移动到最底层
            background.StretchToParentSize(); // 画布铺满整个屏幕

            graphViewChanged = OnGraphViewChanged;
        }

        /// <summary>
        /// 重写右键菜单的逻辑，实现右键菜单创建节点
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            Vector2 mousePos = contentViewContainer.WorldToLocal(evt.mousePosition);

            if (currentTree == null)
            {
                evt.menu.AppendAction("请选择一个BhaviorTreeSO文件",null,DropdownMenuAction.Status.Disabled);
                return;
            }
            
            // 自动识别所有相同类型的节点，并在右键菜单中显示
            // 之后每写一个新的怪物行为，都能在右键菜单中添加
            var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var item in actionNodes)
            {
                if(item.IsAbstract) continue;
                evt.menu.AppendAction($"Action/{item.Name}",_ => CreateNodeAtMousePos(item,mousePos));
            }

            var compositeNodes = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var item in compositeNodes)
            {
                if(item.IsAbstract) continue;
                evt.menu.AppendAction($"Composite/{item.Name}",_ => CreateNodeAtMousePos(item,mousePos));
            }

            var decoratorNodes = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var item in decoratorNodes)
            {
                if (item.IsAbstract) continue;
                evt.menu.AppendAction($"Decorator/{item.Name}",_ => CreateNodeAtMousePos(item,mousePos));
            }
        }

        private void CreateNodeAtMousePos(System.Type type, Vector2 mousePos)
        {
            var node = currentTree.CreateNode(type);
            node.position = mousePos;
            CreateNodeView(node);
        }

        /// <summary>
        /// 重写连线方法
        /// 如果方向和节点都不同，那么就允许连接
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        /// <summary>
        /// 创建一个nodeView的辅助方法
        /// </summary>
        /// <param name="baseNode"></param>
        public void CreateNodeView(BtBaseNode baseNode)
        {
            NodeView nodeView = new NodeView(baseNode);
            AddElement(nodeView);
            nodeView.OnNodeSelected = OnNodeSelected;
        }

        /// <summary>
        /// 监听连线变动的方法
        /// 不论是新拉了一根线，删除了一根线，或者是删除了一个节点，都会触发这个回调
        /// </summary>
        /// <returns></returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            // 如果连接了两个线
            if (change.edgesToCreate != null)
            {
                foreach (var item in change.edgesToCreate)
                {
                    if (item.output.node is NodeView parentView && item.input.node is NodeView childView)
                        currentTree.AddChild(parentView.nodeData, childView.nodeData);
                }
            }

            // 如果删除了
            if (change.elementsToRemove != null)
            {
                foreach (var item in change.elementsToRemove)
                {
                    // 删的是线
                    if (item is Edge edge)
                    {
                        if (edge.output.node is NodeView parentView && edge.input.node is NodeView childrenView)
                            currentTree.RemoveChild(parentView.nodeData, childrenView.nodeData);
                    }

                    // 删的是节点
                    if (item is NodeView nodeView)
                    {
                        currentTree.DeleteNode(nodeView.nodeData);
                    }
                }
            }

            return change;
        }

        /// <summary>
        /// 反序列化绘图，把当前选中的BehaviorTree显示在图中
        /// </summary>
        /// <param name="tree"></param>
        public void PopulateView(BehaviorTreeSO tree)
        {
            this.currentTree = tree;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (currentTree == null) return;

            // 创建唯一的RootNode
            if (currentTree.rootNode == null)
            {
                currentTree.rootNode = currentTree.CreateNode(typeof(RootNode));
                EditorUtility.SetDirty(currentTree);
                AssetDatabase.SaveAssets();
            }
            // 创建所有节点
            currentTree.allNodes.ForEach(node => CreateNodeView(node));

            // 连接所有的连线
            currentTree.allNodes.ForEach(node =>
            {
                NodeView parentView = FindNodeView(node);
                var children = GetChildren(node);

                foreach (var child in children)
                {
                    NodeView childView = FindNodeView(child);
                    if (childView != null && parentView != null)
                    {
                        Edge edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                }
            });
        }

        /// <summary>
        /// 用GUID找每一个孩子（视觉层）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private NodeView FindNodeView(BtBaseNode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        /// <summary>
        /// 找到每一个节点下面的所有孩子（数据层）
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<BtBaseNode> GetChildren(BtBaseNode parent)
        {
            if (parent is CompositeNode compositeNode) return compositeNode.children;
            if (parent is DecoratorNode decoratorNode && decoratorNode.child != null)
                return new List<BtBaseNode> { decoratorNode.child };
            if (parent is RootNode rootNode && rootNode.child != null)
                return new List<BtBaseNode> { rootNode.child };
            return new List<BtBaseNode>();
        }

        
        /// <summary>
        /// 更新节点颜色
        /// </summary>
        public void UpdateNodeStatus()
        {
            nodes.ForEach(n =>
            {
                if (n is NodeView nodeView) nodeView.UpdateState();
            });
        }
    }
}