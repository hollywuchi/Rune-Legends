using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    public class ChaseMoveNode : ActionNode
    {
        protected override void OnStart()
        {
            context.Anim.SetBool(EnemyAnimHush.Run, true);
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null || blackBoard.TargetPlayer == null)
                return BtLifeTimeState.Failure;

            var dirX = blackBoard.TargetPlayer.position.x - context.Transform.position.x;
            var absDirX = Mathf.Abs(dirX);

            if (absDirX < 0.3f)
            {
                context.Motor.Stop();
                context.Anim.SetBool(EnemyAnimHush.Run,false);
                return BtLifeTimeState.Running;
            }
            // 追击时实时面向目标
            context.Anim.SetBool(EnemyAnimHush.Run,true);
            context.Motor.FaceTarget(blackBoard.TargetPlayer);

            // 前方撞墙时刹停等待，防止卡墙持续加速
            if (context.PhysicsCheck != null && context.PhysicsCheck.TouchFrontWall(context.Motor.facingRight))
            {
                context.Motor.Stop();
                context.Anim.SetBool(EnemyAnimHush.Run,false);
                return BtLifeTimeState.Running;
            }

            // 持续向目标追击奔跑
            context.Motor.Move(config != null ? config.chaseSpeed : 5f);

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            context.Anim.SetBool(EnemyAnimHush.Run, false);
            context.Motor.Stop();
        }
    }
}
