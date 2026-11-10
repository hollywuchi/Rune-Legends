using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BehaviorTree.Core
{
    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "BehaviorTreeName", order = 0)]
    public class BehaviorTreeSO : ScriptableObject
    {
        public BtBaseNode rootNode;         // 根节点，这个节点连接所有同其他的节点
        public List<BtBaseNode> allNodes = new();
        public BtLifeTimeState treeState = BtLifeTimeState.Running;
        public Blackboard blackBoard;

        public BtLifeTimeState UpdateTree()
        {
            return rootNode.Evalucate();
        }

        public BehaviorTreeSO Clone()
        {
            BehaviorTreeSO behaviorTreeSo = Instantiate(this);
            behaviorTreeSo.rootNode = rootNode.Clone();
            return behaviorTreeSo;
        }

        public void Bind(EnemyContext context)
        {
            Traverse(rootNode, node =>
            {
                node.context = context;
                node.blackBoard = blackBoard;
            });
        }
        
        /// <summary>
        /// 遍历行为树
        /// </summary>
        /// <param name="node"></param>
        /// <param name="visitor"></param>
        public static void Traverse(BtBaseNode node, Action<BtBaseNode> visitor)
        {
            if (node == null) return;

            visitor(node);

            if (node is DecoratorNode decoratorNode && decoratorNode.child != null)
            {
                Traverse(decoratorNode.child, visitor);
            }

            if (node is CompositeNode compositeNode)
            {
                foreach (var item in compositeNode.children)
                {
                    Traverse(item,visitor);
                }
            }
        }
        
        /// <summary>
        /// 向文件中添加节点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BtBaseNode CreateNode(Type type)
        {
            BtBaseNode node = ScriptableObject.CreateInstance(type) as BtBaseNode;

            node.name = type.Name;
            node.Guid = Guid.NewGuid().ToString();
            
            allNodes.Add(node);

#if UNITY_EDITOR
            // 把新建的Node作为子资产添加进这个SO文件中
            AssetDatabase.AddObjectToAsset(node,this);
            AssetDatabase.SaveAssets();
#endif

            return node;
        }
        
        /// <summary>
        /// 在文件中删除节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(BtBaseNode node)
        {
            allNodes.Remove(node);
#if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
#endif 
        }
        
        /// <summary>
        /// 在数据层把子节点塞到父节点中
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void AddChild(BtBaseNode parent, BtBaseNode child)
        {
            if (parent is CompositeNode compositeNode)
            {
                compositeNode.children.Add(child);
#if UNITY_EDITOR
                EditorUtility.SetDirty(compositeNode);
#endif
            }
            else if (parent is DecoratorNode decoratorNode)
            {
                decoratorNode.child = child;
#if UNITY_EDITOR
                EditorUtility.SetDirty(decoratorNode);
#endif
            }
        }
        
        /// <summary>
        /// 同理，删除
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public void RemoveChild(BtBaseNode parent, BtBaseNode child)
        {
            if (parent is CompositeNode compositeNode)
            {
                compositeNode.children.Remove(child);
#if UNITY_EDITOR
                EditorUtility.SetDirty(compositeNode);
#endif
            }
            else if (parent is DecoratorNode decoratorNode)
            {
                decoratorNode.child = null;
#if UNITY_EDITOR
                EditorUtility.SetDirty(decoratorNode);
#endif
            }
        }
        
        // public 
    }
}