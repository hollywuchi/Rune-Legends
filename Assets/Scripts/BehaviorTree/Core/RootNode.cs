using UnityEngine;

namespace BehaviorTree.Core
{
    public class RootNode : BtBaseNode
    {
        [HideInInspector] public BtBaseNode child;
        
        protected override void OnStart()
        {
            
        }

        protected override BtLifeTimeState OnUpdate()
        {
            return child.Evaluate();
        }

        protected override void OnStop()
        {
        }

        public override BtBaseNode Clone()
        {
            RootNode node = Instantiate(this);
            if (child != null) node.child = child.Clone();
            return node;
        }
    }
}