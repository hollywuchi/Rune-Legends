using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    /// <summary>
    /// Boss 出场演出动画节点（仅执行一次）：
    /// 触发 11_Boss_Intro 动画，期间保持静止与无敌，动画帧事件回调后完成演出。
    /// </summary>
    public class BossIntroNode : ActionNode
    {
        [Tooltip("出场动画无敌保护时长（秒）")]
        public float introInvincibleDuration = 3f;

        [Tooltip("出场动画保底完成时长（若动画未配置 AnimCallBack 事件，超时自动完成）")]
        public float fallbackDuration = 3.5f;

        private float timer;

        protected override void OnStart()
        {
            if (context != null && context.Motor != null)
            {
                context.Motor.attackFinished = false;
                context.Motor.Stop();

                if (blackBoard != null && blackBoard.TargetPlayer != null)
                {
                    context.Motor.FaceTarget(blackBoard.TargetPlayer);
                }
            }

            if (context != null && context.Character != null)
            {
                context.Character.SetInvincibleTime(introInvincibleDuration);
            }

            if (context != null && context.Anim != null)
            {
                context.Anim.SetTrigger(EnemyAnimHush.Intro);
            }

            timer = fallbackDuration;
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (context == null || context.Motor == null)
            {
                if (blackBoard != null) blackBoard.HasPlayedIntro = true;
                return BtLifeTimeState.Success;
            }

            timer -= Time.deltaTime;

            if (context.Motor.attackFinished || timer <= 0f)
            {
                if (blackBoard != null) blackBoard.HasPlayedIntro = true;
                return BtLifeTimeState.Success;
            }

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            if (blackBoard != null)
            {
                blackBoard.HasPlayedIntro = true;
            }

            if (context != null && context.Motor != null)
            {
                context.Motor.attackFinished = false;
            }
        }
    }
}
