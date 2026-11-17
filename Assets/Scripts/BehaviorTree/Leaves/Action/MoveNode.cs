using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    public class MoveNode : ActionNode
    {
        private int activeAnimHush;


        protected override void OnStart()
        {
            activeAnimHush = blackBoard.TargetPlayer != null ? EnemyAnimHush.Run : EnemyAnimHush.Walk;
            context.Anim.SetBool(activeAnimHush, true);
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;

            if (context.PhysicsCheck.TouchFrontWall(context.Motor.facingRight))
                return BtLifeTimeState.Success;

            context.Motor.Move(blackBoard.TargetPlayer == null
                ? config.normalSpeed
                : config.chaseSpeed);

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            if (activeAnimHush != 0)
                context.Anim.SetBool(activeAnimHush, false);
        }
    }
}