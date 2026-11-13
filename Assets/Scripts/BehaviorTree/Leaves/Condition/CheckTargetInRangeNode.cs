using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class CheckTargetInRangeNode : ActionNode
    {
        private float loseTargetTime;
        private float loseTargetTimer;

        protected override void OnStart()
        {
            if (context.Config != null)
                loseTargetTime = context.Config.loseTargetTime;
        }

        protected override BtLifeTimeState OnUpdate()
        {
            var playerTrans = context.Motor.FindPlayer();
            if (playerTrans == null) return BtLifeTimeState.Failure;

            loseTargetTimer = loseTargetTime;
            if (blackBoard != null)
                blackBoard.targetPlayer = playerTrans;

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