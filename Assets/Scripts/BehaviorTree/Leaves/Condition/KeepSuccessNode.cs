using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class KeepSuccessNode : DecoratorNode
    {
        private float timer;
        
        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (child == null) return BtLifeTimeState.Failure;

            var state = child.Evaluate();

            if (state == BtLifeTimeState.Success)
            {
                timer = config == null ? 3f : config.loseTargetTime;
                return BtLifeTimeState.Success;
            }

            if (state == BtLifeTimeState.Failure && timer > 0)
            {
                timer -= Time.deltaTime;
                return BtLifeTimeState.Success;
            }

            return state;
        }

        protected override void OnStop()
        {
        }
    }
}