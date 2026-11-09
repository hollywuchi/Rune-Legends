using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Core
{
    [CreateAssetMenu(fileName = "BehaviorTree", menuName = "BehaviorTreeName", order = 0)]
    public class BehaviorTreeSO : ScriptableObject
    {
        public BtBaseNode rootNode;
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
    }
}