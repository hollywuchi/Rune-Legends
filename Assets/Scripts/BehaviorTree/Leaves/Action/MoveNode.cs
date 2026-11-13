using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    public class MoveNode : ActionNode
    {
        protected override void OnStart()
        {
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;
            context.Motor.Move(blackBoard.targetPlayer == null
                ? context.Config.normalSpeed
                : context.Config.chaseSpeed);
            
            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
        }
    }
}