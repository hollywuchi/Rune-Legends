using DG.Tweening;
using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    /// <summary>
    /// Boss 残影闪避行为节点：
    /// 触发 Dodge 动画，施加短暂无敌帧，向远离玩家方向（或前方穿身方向）快速位移。
    /// </summary>
    public class BossDodgeNode : ActionNode
    {
        [Header("闪避参数")]
        [Tooltip("闪避速度")]
        public float dodgeSpeed = 12f;

        [Tooltip("闪避期间无敌时长（秒）")]
        public float invincibleDuration = 0.6f;

        [Tooltip("闪避位移持续时长（秒，超过此时长停止水平位移）")]
        public float dashDuration = 0.3f;

        [Tooltip("超时保底（秒）")]
        public float fallbackTimeout = 1.0f;

        [Tooltip("是否强制背对玩家后撤（true 则面向玩家向后撤步，false 则向前穿身突进）")]
        public bool awayFromPlayer = true;

        private float timer;
        private float dashTimer;

        protected override void OnStart()
        {
            if (context == null || context.Motor == null) return;

            context.Motor.attackFinished = false;

            if (blackBoard != null && blackBoard.TargetPlayer != null)
            {
                context.Motor.FaceTarget(blackBoard.TargetPlayer);
            }

            if (context.Character != null)
            {
                context.Character.SetInvincibleTime(invincibleDuration);
            }

            float dirSign = awayFromPlayer ? -1f : 1f;
            context.Motor.ForceMoveVelocity(dodgeSpeed, dashDuration, dirSign, Ease.OutCubic);

            if (context.Anim != null)
            {
                context.Anim.SetTrigger(EnemyAnimHush.Dodge);
            }

            timer = fallbackTimeout;
            dashTimer = dashDuration;
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (context == null || context.Motor == null)
            {
                return BtLifeTimeState.Failure;
            }

            timer -= Time.deltaTime;

            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    context.Motor.Stop();
                }
            }

            if (context.Motor.attackFinished || timer <= 0f)
            {
                context.Motor.Stop();
                return BtLifeTimeState.Success;
            }

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            if (context != null && context.Motor != null)
            {
                context.Motor.attackFinished = false;
                context.Motor.Stop();
            }
        }
    }
}
