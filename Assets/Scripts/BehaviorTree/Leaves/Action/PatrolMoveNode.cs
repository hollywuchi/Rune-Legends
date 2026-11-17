using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    public class PatrolMoveNode : ActionNode
    {
        private float patrolTimer;

        protected override void OnStart()
        {
            context.Anim.SetBool(EnemyAnimHush.Walk, true);
            patrolTimer = config != null ? config.patrolTime : 5f;
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (blackBoard == null) return BtLifeTimeState.Failure;

            // 1. 撞墙退出，触发后续的等待和转身
            if (context.PhysicsCheck != null && context.PhysicsCheck.TouchFrontWall(context.Motor.facingRight))
                return BtLifeTimeState.Success;

            // 2. 悬崖边上退出，触发后续的等待和转身
            if (context.PhysicsCheck != null && context.PhysicsCheck.IsFrontGround(context.Motor.facingRight))
                return BtLifeTimeState.Success;

            // 3. 巡逻计时超时退出（在大平地上来回巡逻）
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0f)
                return BtLifeTimeState.Success;

            // 正常巡逻行走
            context.Motor.Move(config != null ? config.normalSpeed : 2f);

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            context.Anim.SetBool(EnemyAnimHush.Walk, false);
            context.Motor.Stop();
        }
    }
}
