using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class CheckTargetInRangeNode : ActionNode
    {
        private float loseTargetTimer;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            var playerTrans = context.Motor.FindPlayer();
            
            if (playerTrans != null)
            {
                loseTargetTimer = config != null ? config.loseTargetTime : 1f;
                if (blackBoard != null)
                    blackBoard.targetPlayer = playerTrans;
                return BtLifeTimeState.Success;
            }

            if (loseTargetTimer > 0)
            {
                loseTargetTimer -= Time.deltaTime;
                return BtLifeTimeState.Success;
            }

            if (blackBoard != null) blackBoard.targetPlayer = null; 
            
            return BtLifeTimeState.Failure;
        }

        protected override void OnStop()
        {
        }
    }
}