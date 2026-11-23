using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorTree.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.BehaviorTreeEditor
{
    public class BehaviorTreeView : GraphView
    {
        private const string ClipboardMagicHeader = "BT_NODES_V1:";
        private static readonly Vector2 PasteOffsetStep = new Vector2(30f, 30f);

        private BehaviorTreeSO currentTree;
        private string _lastPastedClipboardData;
        private int _consecutivePasteCount;

        public Action<NodeView> OnNodeSelected;

        [Serializable]
        private class ClipboardData
        {
            public string sourceTreeId;
            public List<SerializedNodeData> nodes = new();
            public List<SerializedEdgeData> edges = new();
        }

        [Serializable]
        private class SerializedNodeData
        {
            public string originalGuid;
            public string assemblyQualifiedTypeName;
            public string nodeName;
            public Vector2 position;
            public string jsonData;
        }

        [Serializable]
        private class SerializedEdgeData
        {
            public string parentGuid;
            public string childGuid;
            public int childOrder;
        }

        public BehaviorTreeView()
        {
            focusable = true;

            this.AddManipulator(new ContentZoomer()); // 滚轮缩放
            this.AddManipulator(new ContentDragger()); // 右键或中键移动
            this.AddManipulator(new SelectionDragger()); // 选中拖动
            this.AddManipulator(new RectangleSelector()); // 矩形选框

            GridBackground background = new GridBackground();
            Insert(0, background); // 移动到最底层
            background.StretchToParentSize(); // 画布铺满整个屏幕

            graphViewChanged = OnGraphViewChanged;
            serializeGraphElements = OnSerializeGraphElements;
            canPasteSerializedData = OnCanPasteSerializedData;
            unserializeAndPaste = OnUnserializeAndPaste;
        }

        /// <summary>
        /// 重写右键菜单的逻辑，实现右键菜单创建节点与复制粘贴
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            Vector2 mousePos = contentViewContainer.WorldToLocal(evt.mousePosition);

            if (currentTree == null)
            {
                evt.menu.AppendAction("请选择一个BhaviorTreeSO文件", null, DropdownMenuAction.Status.Disabled);
                return;
            }

            List<NodeView> copyTargets = GetContextCopyTargets(evt.target);
            DropdownMenuAction.Status copyStatus = copyTargets.Count > 0
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
            DropdownMenuAction.Status pasteStatus = OnCanPasteSerializedData(EditorGUIUtility.systemCopyBuffer)
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;

            evt.menu.AppendAction("复制节点", _ =>
            {
                string serialized = SerializeNodeViews(copyTargets);
                if (!string.IsNullOrEmpty(serialized))
                {
                    EditorGUIUtility.systemCopyBuffer = serialized;
                }
            }, copyStatus);

            evt.menu.AppendAction("粘贴节点", _ =>
            {
                PasteSerializedData("Paste", EditorGUIUtility.systemCopyBuffer, mousePos);
            }, pasteStatus);

            evt.menu.AppendAction("克隆节点", _ =>
            {
                string serialized = SerializeNodeViews(copyTargets);
                if (!string.IsNullOrEmpty(serialized))
                {
                    PasteSerializedData("Duplicate", serialized, null);
                }
            }, copyStatus);

            evt.menu.AppendSeparator();

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

        private List<NodeView> GetContextCopyTargets(IEventHandler eventTarget)
        {
            NodeView targetNodeView = eventTarget as NodeView ??
                                      (eventTarget as VisualElement)?.GetFirstAncestorOfType<NodeView>();

            if (targetNodeView != null && !selection.Contains(targetNodeView))
            {
                if (targetNodeView.nodeData != null && !(targetNodeView.nodeData is RootNode))
                {
                    return new List<NodeView> { targetNodeView };
                }

                return new List<NodeView>();
            }

            return selection
                .OfType<NodeView>()
                .Where(nv => nv != null && nv.nodeData != null && !(nv.nodeData is RootNode))
                .ToList();
        }

        private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            if (elements == null) return string.Empty;
            return SerializeNodeViews(elements.OfType<NodeView>());
        }

        private string SerializeNodeViews(IEnumerable<NodeView> nodeViews)
        {
            if (currentTree == null || nodeViews == null) return string.Empty;

            List<BtBaseNode> nodesToCopy = new();
            HashSet<string> selectedGuids = new();

            foreach (NodeView nodeView in nodeViews)
            {
                if (nodeView == null || nodeView.nodeData == null || nodeView.nodeData is RootNode) continue;

                BtBaseNode nodeData = nodeView.nodeData;
                if (string.IsNullOrEmpty(nodeData.Guid) || !selectedGuids.Add(nodeData.Guid)) continue;

                nodesToCopy.Add(nodeData);
            }

            if (nodesToCopy.Count == 0) return string.Empty;

            _lastPastedClipboardData = null;
            _consecutivePasteCount = 0;

            ClipboardData clipboardData = new ClipboardData
            {
                sourceTreeId = GetTreeIdentifier(currentTree)
            };

            foreach (BtBaseNode node in nodesToCopy)
            {
                clipboardData.nodes.Add(new SerializedNodeData
                {
                    originalGuid = node.Guid,
                    assemblyQualifiedTypeName = node.GetType().AssemblyQualifiedName,
                    nodeName = node.name,
                    position = node.position,
                    jsonData = EditorJsonUtility.ToJson(node)
                });
            }

            HashSet<string> recordedEdges = new();
            foreach (BtBaseNode parent in nodesToCopy)
            {
                List<BtBaseNode> children = GetChildren(parent);
                for (int i = 0; i < children.Count; i++)
                {
                    BtBaseNode child = children[i];
                    if (child == null || string.IsNullOrEmpty(child.Guid) || !selectedGuids.Contains(child.Guid))
                    {
                        continue;
                    }

                    string edgeKey = $"{parent.Guid}->{child.Guid}";
                    if (!recordedEdges.Add(edgeKey)) continue;

                    clipboardData.edges.Add(new SerializedEdgeData
                    {
                        parentGuid = parent.Guid,
                        childGuid = child.Guid,
                        childOrder = i
                    });
                }
            }

            return ClipboardMagicHeader + JsonUtility.ToJson(clipboardData);
        }

        private bool OnCanPasteSerializedData(string data)
        {
            return currentTree != null && TryParseClipboardData(data, out _);
        }

        private void OnUnserializeAndPaste(string operationName, string data)
        {
            PasteSerializedData(operationName, data, null);
        }

        private void PasteSerializedData(string operationName, string data, Vector2? pastePosition)
        {
            if (currentTree == null || !TryParseClipboardData(data, out ClipboardData clipboardData)) return;

            Vector2 minPos = clipboardData.nodes[0].position;
            Vector2 maxPos = clipboardData.nodes[0].position;
            for (int i = 1; i < clipboardData.nodes.Count; i++)
            {
                Vector2 pos = clipboardData.nodes[i].position;
                minPos = Vector2.Min(minPos, pos);
                maxPos = Vector2.Max(maxPos, pos);
            }

            Vector2 positionDelta;
            if (pastePosition.HasValue)
            {
                positionDelta = pastePosition.Value - minPos;
            }
            else if (string.Equals(operationName, "Duplicate", StringComparison.Ordinal))
            {
                positionDelta = PasteOffsetStep;
            }
            else
            {
                if (string.Equals(_lastPastedClipboardData, data, StringComparison.Ordinal))
                {
                    _consecutivePasteCount++;
                }
                else
                {
                    _lastPastedClipboardData = data;
                    _consecutivePasteCount = 1;
                }

                if (string.Equals(clipboardData.sourceTreeId, GetTreeIdentifier(currentTree), StringComparison.Ordinal))
                {
                    positionDelta = PasteOffsetStep * _consecutivePasteCount;
                }
                else
                {
                    Vector2 viewportCenter = contentViewContainer.WorldToLocal(worldBound.center);
                    Vector2 groupCenter = (minPos + maxPos) * 0.5f;
                    positionDelta = (viewportCenter - groupCenter) + PasteOffsetStep * (_consecutivePasteCount - 1);
                }
            }

            Dictionary<string, BtBaseNode> oldGuidToNewNode = new();
            Dictionary<string, NodeView> oldGuidToNewView = new();
            List<NodeView> createdViews = new();

            foreach (SerializedNodeData nodeDto in clipboardData.nodes)
            {
                Type nodeType = ResolveNodeType(nodeDto.assemblyQualifiedTypeName);
                if (nodeType == null || nodeType.IsAbstract || !typeof(BtBaseNode).IsAssignableFrom(nodeType) || nodeType == typeof(RootNode))
                {
                    Debug.LogWarning($"[BehaviorTreeEditor] 跳过无法粘贴的节点类型: {nodeDto.assemblyQualifiedTypeName}");
                    continue;
                }

                BtBaseNode newNode = ScriptableObject.CreateInstance(nodeType) as BtBaseNode;
                if (newNode == null) continue;

                if (!string.IsNullOrEmpty(nodeDto.jsonData))
                {
                    EditorJsonUtility.FromJsonOverwrite(nodeDto.jsonData, newNode);
                }

                newNode.name = string.IsNullOrEmpty(nodeDto.nodeName) ? nodeType.Name : nodeDto.nodeName;
                newNode.Guid = System.Guid.NewGuid().ToString();
                newNode.position = nodeDto.position + positionDelta;
                newNode.State = BtLifeTimeState.Running;
                newNode.Started = false;
                newNode.LastEvaluateTime = -1f;
                newNode.context = null;
                newNode.blackBoard = null;
                newNode.config = null;

                if (newNode is CompositeNode composite)
                {
                    composite.children = new List<BtBaseNode>();
                }
                else if (newNode is DecoratorNode decorator)
                {
                    decorator.child = null;
                }

                currentTree.AddExistingNode(newNode, saveAssets: false);
                NodeView newView = CreateNodeView(newNode);

                if (!string.IsNullOrEmpty(nodeDto.originalGuid))
                {
                    oldGuidToNewNode[nodeDto.originalGuid] = newNode;
                    oldGuidToNewView[nodeDto.originalGuid] = newView;
                }

                createdViews.Add(newView);
            }

            if (clipboardData.edges != null)
            {
                foreach (SerializedEdgeData edgeDto in clipboardData.edges.OrderBy(e => e.childOrder))
                {
                    if (!oldGuidToNewNode.TryGetValue(edgeDto.parentGuid, out BtBaseNode parentNode) ||
                        !oldGuidToNewNode.TryGetValue(edgeDto.childGuid, out BtBaseNode childNode) ||
                        !oldGuidToNewView.TryGetValue(edgeDto.parentGuid, out NodeView parentView) ||
                        !oldGuidToNewView.TryGetValue(edgeDto.childGuid, out NodeView childView))
                    {
                        continue;
                    }

                    if (parentView.output == null || childView.input == null) continue;

                    currentTree.AddChild(parentNode, childNode);
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }

            if (createdViews.Count > 0)
            {
                EditorUtility.SetDirty(currentTree);
                if (EditorUtility.IsPersistent(currentTree))
                {
                    AssetDatabase.SaveAssets();
                }

                ClearSelection();
                foreach (NodeView view in createdViews)
                {
                    AddToSelection(view);
                }
            }
        }

        private static bool TryParseClipboardData(string data, out ClipboardData clipboardData)
        {
            clipboardData = null;
            if (string.IsNullOrEmpty(data) || !data.StartsWith(ClipboardMagicHeader, StringComparison.Ordinal))
            {
                return false;
            }

            string json = data.Substring(ClipboardMagicHeader.Length);
            if (string.IsNullOrEmpty(json)) return false;

            try
            {
                clipboardData = JsonUtility.FromJson<ClipboardData>(json);
            }
            catch (Exception)
            {
                clipboardData = null;
                return false;
            }

            return clipboardData != null && clipboardData.nodes != null && clipboardData.nodes.Count > 0;
        }

        private static Type ResolveNodeType(string assemblyQualifiedTypeName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedTypeName)) return null;

            Type type = Type.GetType(assemblyQualifiedTypeName);
            if (type != null) return type;

            string fullName = assemblyQualifiedTypeName;
            int commaIndex = assemblyQualifiedTypeName.IndexOf(',');
            if (commaIndex >= 0)
            {
                fullName = assemblyQualifiedTypeName.Substring(0, commaIndex).Trim();
            }

            foreach (Type candidate in TypeCache.GetTypesDerivedFrom<BtBaseNode>())
            {
                if (candidate.AssemblyQualifiedName == assemblyQualifiedTypeName || candidate.FullName == fullName)
                {
                    return candidate;
                }
            }

            return null;
        }

        private static string GetTreeIdentifier(BehaviorTreeSO tree)
        {
            if (tree == null) return string.Empty;

            string assetPath = AssetDatabase.GetAssetPath(tree);
            if (!string.IsNullOrEmpty(assetPath))
            {
                string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                if (!string.IsNullOrEmpty(assetGuid))
                {
                    return assetGuid;
                }
            }

            return tree.GetInstanceID().ToString();
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
        public NodeView CreateNodeView(BtBaseNode baseNode)
        {
            NodeView nodeView = new NodeView(baseNode);
            AddElement(nodeView);
            nodeView.OnNodeSelected = OnNodeSelected;
            return nodeView;
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
                    {
                        currentTree.AddChild(parentView.nodeData, childView.nodeData);
                        parentView.SortChildren(parentView.nodeData);
                    }
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
            _lastPastedClipboardData = null;
            _consecutivePasteCount = 0;
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