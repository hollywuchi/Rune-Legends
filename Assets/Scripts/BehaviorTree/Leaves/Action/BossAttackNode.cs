using UnityEngine;

namespace BehaviorTree.Leaves.Action
{
    /// <summary>
    /// Boss 通用攻击行为节点：
    /// 用于驱动 Attack1、Attack2、Attack3(重劈)、AttackCombo(连斩)、DodgeCounter(上挑)、DodgeCounterPierce(突刺)。
    /// 支持配置出招霸体、自动转向与前冲初速度。
    /// </summary>
    public class BossAttackNode : ActionNode
    {
        [Header("动画配置")]
        [Tooltip("Animator 中的 Trigger 参数名，例如：Attack1, Attack2, Attack3, AttackCombo, DodgeCounter, DodgeCounterPierce")]
        public string animTriggerName = "Attack1";

        [Header("动作参数")]
        [Tooltip("起手时是否自动转向面对玩家")]
        public bool faceTargetOnStart = true;

        [Tooltip("出招期间是否开启霸体（不可被普通攻击打断）")]
        public bool superArmorDuringAttack = true;

        [Tooltip("起手前冲速度（向当前面朝方向位移，突刺/重劈可配置）")]
        public float lungeSpeed = 0f;

        [Tooltip("防卡死超时时长（若动画未配置 AnimCallBack 事件，超时自动完成）")]
        public float fallbackTimeout = 4f;

        private float timeoutTimer;
        private bool originalSuperArmor;

        protected override void OnStart()
        {
            if (context == null || context.Motor == null) return;

            context.Motor.attackFinished = false;
            context.Motor.Stop();

            if (faceTargetOnStart && blackBoard != null && blackBoard.TargetPlayer != null)
            {
                context.Motor.FaceTarget(blackBoard.TargetPlayer);
            }

            if (superArmorDuringAttack)
            {
                originalSuperArmor = context.Motor.hasSuperArmor;
                context.Motor.hasSuperArmor = true;
            }

            if (lungeSpeed > 0f)
            {
                context.Motor.Dash(lungeSpeed, 1f);
            }

            if (context.Anim != null && !string.IsNullOrEmpty(animTriggerName))
            {
                context.Anim.SetTrigger(animTriggerName);
            }

            timeoutTimer = fallbackTimeout;
        }

        protected override BtLifeTimeState OnUpdate()
        {
            if (context == null || context.Motor == null)
            {
                return BtLifeTimeState.Failure;
            }

            timeoutTimer -= Time.deltaTime;

            if (context.Motor.attackFinished || timeoutTimer <= 0f)
            {
                return BtLifeTimeState.Success;
            }

            return BtLifeTimeState.Running;
        }

        protected override void OnStop()
        {
            if (context != null && context.Motor != null)
            {
                context.Motor.attackFinished = false;
                if (superArmorDuringAttack)
                {
                    context.Motor.hasSuperArmor = originalSuperArmor;
                }
            }
        }
    }
}
