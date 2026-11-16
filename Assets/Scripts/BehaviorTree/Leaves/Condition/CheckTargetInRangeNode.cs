using UnityEngine;

namespace BehaviorTree.Leaves.Condition
{
    public class CheckTargetInRangeNode : ActionNode
    {
        public Vector2 offset;
        public Vector2 checkSize;

        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            context.Motor.offset = offset;
            context.Motor.checkSize = checkSize;
            var playerTrans = context.Motor.FindPlayer();
            
            if (playerTrans != null)
            {
                // loseTargetTimer = config != null ? config.loseTargetTime : 1f;
                if (blackBoard != null)
                    blackBoard.targetPlayer = playerTrans;
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